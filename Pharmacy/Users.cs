using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmacy
{
    public interface IUsers
    {
        string Surname { get; set; }
        string Name { get; set; }
        string PhoneNumber { get; set; }
        string Email { get; set; }
        string Login { get; set; }
        string Password { get; set; }
        bool IsAdministrator { get; set; }
    }

    public class Users : IUsers
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsAdministrator { get; set; }

        public Users() { }
        
        public Users(string surname, string name, string phoneNumber, string email, string login, string password, bool isAdministrator)
        {
            Surname = surname;
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
            Login = login;
            Password = password;
            IsAdministrator = isAdministrator;
        }

        public static List<Users> LoadUsersFromJson()
        {
               List<Users> users = new List<Users>();
               const string jsonFileName = "Users.json";
            try
            {
                if (File.Exists(jsonFileName))
                {
                    string json = File.ReadAllText(jsonFileName);
                    users = JsonConvert.DeserializeObject<List<Users>>(json);
                }
                else
                {
                    users = new List<Users>();
                }
                return users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні користувачів: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public static Users AuthenticateUser(string login, string password)
        {

            List<Users> users = new List<Users>();
            users = LoadUsersFromJson();

            foreach (Users user in users)
            {
                if (user.Login == login)
                {
                    return user;
                }
            }
            return null;
        }

        public static bool UserExistsLogin(string login)
        {

            List<Users> users = new List<Users>();
            users = LoadUsersFromJson();

            foreach (Users user in users)
            {
                if (user.Login == login)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool UserExistsPhoneNumber(string phoneNumber)
        {

            List<Users> users = new List<Users>();

            users = LoadUsersFromJson();

            foreach (Users user in users)
            {
                if (user.PhoneNumber == phoneNumber)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsCorrectPassword(Users user, string password)
        {
            string hashedPassword = ComputeHash(password); 
            return user.Password == hashedPassword; 
        }

        public static string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static void SaveUsersToJson(Users newUser)
        {
            List<Users> users = LoadUsersFromJson();
            const string jsonFileName = "Users.json";
            users.Add(new Users
            {
                Surname = newUser.Surname,
                Name = newUser.Name,
                PhoneNumber = newUser.PhoneNumber,
                Email = newUser.Email,
                Login = newUser.Login,
                Password = ComputeHash(newUser.Password),
                IsAdministrator = newUser.IsAdministrator
            });
            try
            {
                string json = JsonConvert.SerializeObject(users);
                File.WriteAllText(jsonFileName, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні користувачів: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void UpdateUsersToJson(List<Users> users)
        {
            const string jsonFileName = "Users.json";
            try
            {
                string json = JsonConvert.SerializeObject(users);
                File.WriteAllText(jsonFileName, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні користувачів: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
