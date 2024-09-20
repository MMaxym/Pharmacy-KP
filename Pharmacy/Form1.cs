using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Reflection.Emit;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Security.Cryptography;

namespace Pharmacy
{
    public partial class Form1 : Form
    {
        private const string jsonFileName = "Users.json";

        public Form1()
        {
            InitializeComponent();
            Users.LoadUsersFromJson();

            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = this.MaximumSize;
            this.MaximumSize = this.MaximumSize;

            textBox6.TextChanged += textBox6_TextChanged;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox6.Text;

            DialogResult createAccount = DialogResult.None;
            DialogResult exit = DialogResult.None;

            Users loggedInUser = Users.AuthenticateUser(login, password);

            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Введіть логін!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
            }
            else if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введіть пароль!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox6.Focus();
            }
            else
            {
                if (loggedInUser != null)
                {
                    if (!Users.IsCorrectPassword(loggedInUser, password))
                    {
                        MessageBox.Show("Хибний пароль!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox6.Focus();
                        return;
                    }
                    else
                    {
                        if (loggedInUser.IsAdministrator)
                        {
                            MessageBox.Show($"Ви авторизувались як {loggedInUser.Name} {loggedInUser.Surname}", "Вхід в акаунт", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Form2 form2 = new Form2();
                            form2.UserName = $"{loggedInUser.Name} {loggedInUser.Surname}";
                            form2.ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show($"Ви авторизувались як {loggedInUser.Name} {loggedInUser.Surname}", "Вхід в акаунт", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Form3 form3 = new Form3();
                            form3.UserName = $"{loggedInUser.Name} {loggedInUser.Surname}";
                            form3.UserPhone = loggedInUser.PhoneNumber;
                            form3.UserLogin = loggedInUser.Login;
                            form3.ShowDialog();
                            this.Close();
                        }
                    }
                }
                else
                {
                    createAccount = MessageBox.Show("Такого користувача не існує!!!\nБажаєте створити новий акаунт?", "Помилка авторизації",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (createAccount == DialogResult.Yes)
                    {
                        label1.Visible = false;
                        label6.Visible = false;
                        label8.Visible = false;
                        label2.Visible = true;
                        label3.Visible = true;
                        label4.Visible = true;
                        label5.Visible = true;
                        label9.Visible = true;
                        label10.Visible = true;
                        label11.Visible = true;
                        label12.Visible = true;
                        textBox1.Visible = false;
                        textBox6.Visible = false;
                        checkBox2.Visible = false;
                        pictureBox1.Visible = false;
                        pictureBox3.Visible = true;
                        textBox2.Visible = true;
                        textBox3.Visible = true;
                        textBox4.Visible = true;
                        textBox5.Visible = true;
                        textBox7.Visible = true;
                        textBox8.Visible = true;
                        textBox9.Visible = true;
                        button1.Visible = false;
                        button2.Visible = false;
                        button5.Visible = false;
                        button3.Visible = true;
                        button4.Visible = true;

                        textBox2.Clear();
                        textBox3.Clear();
                        textBox4.Clear();
                        textBox5.Clear();
                        textBox7.Clear();
                        textBox8.Clear();
                        textBox9.Clear();
                    }
                    else if (createAccount == DialogResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        exit = MessageBox.Show("Бажаєте вийти з програми?", "Вихід",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (exit == DialogResult.Yes)
                        {
                            MessageBox.Show("Дякую за візит!\nДо зустрічі!", "Вихід", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Будь ласка, введіть прізвище!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Будь ласка, введіть ім'я!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Будь ласка, введіть номер телефону!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Будь ласка, введіть email!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox5.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(textBox7.Text))
            {
                MessageBox.Show("Будь ласка, введіть логін!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox7.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(textBox8.Text))
            {
                MessageBox.Show("Будь ласка, введіть пароль!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox8.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(textBox9.Text))
            {
                MessageBox.Show("Будь ласка, введіть пароль повторно!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox9.Focus();
                return;
            }
            else if (textBox7.Text.Length < 4)
            {
                MessageBox.Show("Логін повинен бути не коротше 4 символів!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox7.Focus();
                return;
            }
            else if(textBox8.Text.Length < 4)
            {
                MessageBox.Show("Пароль повинен бути не коротше 4 символів/цифр!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox8.Focus();
                return;
            }
            else if (textBox4.Text.Length < 10)
            {
                MessageBox.Show("Будь ласка, введіть номер телефону повністю!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Focus();
                return;
            }
            else if (!textBox5.Text.Contains("@"))
            {
                MessageBox.Show("Будь ласка, введіть коректний email !\nEmail обов'язково має містити '@'", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox5.Focus();
                return;
            }
            else if (textBox8.Text != textBox9.Text)
            {
                MessageBox.Show("Паролі не співпадають !!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox9.Focus();
                return;
            }
            else if (Users.UserExistsLogin(textBox7.Text))
            {
                MessageBox.Show("Користувач з таким логіном вже існує !!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox7.Focus();
            }
            else if (Users.UserExistsPhoneNumber(textBox4.Text))
            {
                MessageBox.Show("Користувач з таким номером телефону вже існує !!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox8.Focus();
            }
            else
            {
                Users newUser = new Users
                {
                    Surname = textBox2.Text,
                    Name = textBox3.Text,
                    PhoneNumber = textBox4.Text,
                    Email = textBox5.Text,
                    Login = textBox7.Text,
                    Password = textBox8.Text,
                    IsAdministrator = false
                };


                Users.SaveUsersToJson(newUser);

                MessageBox.Show($"Акаунт для {newUser.Name} {newUser.Surname} створено успішно!", "Акаунт створено", MessageBoxButtons.OK, MessageBoxIcon.Information);

                label1.Visible = true;
                label6.Visible = true;
                label8.Visible = true;
                label2.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                textBox1.Visible = true;
                textBox6.Visible = true;
                checkBox2.Visible = true;
                pictureBox1.Visible = true;
                pictureBox3.Visible = false;
                textBox2.Visible = false;
                textBox3.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
                textBox7.Visible = false;
                textBox8.Visible = false;
                textBox9.Visible = false;
                button1.Visible = true;
                button2.Visible = true;
                button5.Visible = true;
                button3.Visible = false;
                button4.Visible = false;

                textBox1.Clear();
                textBox6.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Visible = false;
            label6.Visible = false;
            label8.Visible = false;
            label2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            label9.Visible = true;
            label10.Visible = true;
            label11.Visible = true;
            label12.Visible = true;
            textBox1.Visible = false;
            textBox6.Visible = false;
            checkBox2.Visible = false;
            pictureBox1.Visible = false;
            pictureBox3.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = true;
            textBox5.Visible = true;
            textBox7.Visible = true;
            textBox8.Visible = true;
            textBox9.Visible = true;
            button1.Visible = false;
            button2.Visible = false;
            button5.Visible = false;
            button3.Visible = true;
            button4.Visible = true;

            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();

            textBox2.Focus();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (textBox2.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox3.Focus();
                textBox3.SelectAll();
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (textBox3.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox4.Focus();
                textBox4.SelectAll();
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (textBox4.Text.Length >= 10 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox5.Focus();
                textBox5.SelectAll();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Visible = true;
            label6.Visible = true;
            label8.Visible = true;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            textBox1.Visible = true;
            textBox6.Visible = true;
            checkBox2.Visible = true;
            pictureBox1.Visible = true;
            pictureBox3.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox7.Visible = false;
            textBox8.Visible = false;
            textBox9.Visible = false;
            button1.Visible = true;
            button2.Visible = true;
            button5.Visible = true;
            button3.Visible = false;
            button4.Visible = false;

            textBox1.Clear();
            textBox6.Clear();

            textBox1.Focus();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            Form2 form2 = new Form2();
            form2.Close();
            Form3 form3 = new Form3();
            form3.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox1.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox6.Focus();
                textBox6.SelectAll();
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox6.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox5.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox7.Focus();
                textBox7.SelectAll();
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox7.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox8.Focus();
                textBox8.SelectAll();
            }
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox8.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox9.Focus();
                textBox9.SelectAll();
            }
        }

        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox9.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                button3_Click(sender, e);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) 
            {
                textBox6.PasswordChar = '\0';
            }
            else
            {
                textBox6.PasswordChar = '●';
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                textBox6.PasswordChar = '\0';
            }
            else
            {
                textBox6.PasswordChar = '●';
            }
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(76, 136, 74);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(76, 136, 74);
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(51, 91,49);
            button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 91, 49);
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(51, 91, 49);
            button3.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 91, 49);
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(76, 136, 74);
            button3.FlatAppearance.MouseOverBackColor = Color.FromArgb(76, 136, 74);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            label13.Visible = true;
            label14.Visible = true;
            label15.Visible = true;
            label16.Visible = true;
            textBox10.Visible = true;
            textBox11.Visible = true;
            textBox12.Visible = true;
            button6.Visible = true;
            button7.Visible = true;

            label1.Visible = false;
            label6.Visible = false;
            label8.Visible = false;
            textBox1.Visible = false;
            textBox6.Visible = false;
            checkBox2.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button5.Visible = false;

            textBox1.Clear();
            textBox6.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            label13.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            label16.Visible = false;
            textBox10.Visible = false;
            textBox11.Visible = false;
            textBox12.Visible = false;
            button6.Visible = false;
            button7.Visible = false;

            label1.Visible = true;
            label6.Visible = true;
            label8.Visible = true;
            textBox1.Visible = true;
            textBox6.Visible = true;
            checkBox2.Visible = true;
            button1.Visible = true;
            button2.Visible = true; 
            button5.Visible = true;

            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();

            textBox1.Focus();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string loginToUpdate = textBox11.Text;
            string newPassword = textBox10.Text;

            if (string.IsNullOrWhiteSpace(textBox11.Text))
            {
                MessageBox.Show("Будь ласка, введіть логін!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox11.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(textBox10.Text))
            {
                MessageBox.Show("Будь ласка, введіть пароль!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox10.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(textBox12.Text))
            {
                MessageBox.Show("Будь ласка, введіть пароль повторно!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox12.Focus();
                return;
            }
            else if (textBox10.Text.Length < 4)
            {
                MessageBox.Show("Пароль повинен бути не коротше 4 символів/цифр!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox10.Focus();
                return;
            }
            else if (textBox10.Text != textBox12.Text)
            {
                MessageBox.Show("Паролі не співпадають !!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox12.Focus();
                return;
            }

            List<Users> users = new List<Users>();

            users = Users.LoadUsersFromJson();

            Users userToUpdate = null;
            foreach (Users user in users)
            {
                if (user.Login == loginToUpdate)
                {
                    userToUpdate = user;
                    break;
                }
            }

            if (userToUpdate == null)
            {
                MessageBox.Show($"Користувач з логіном '{loginToUpdate}' не існує !!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox11.Focus();
                return;
            }

            string hashedPassword = Users.ComputeHash(newPassword);
            userToUpdate.Password = hashedPassword;
            Users.UpdateUsersToJson(users);

            MessageBox.Show($"Пароль для користувача з логіном '{loginToUpdate}' успішно оновлено !!!", "Оновлення паролю", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            label13.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            label16.Visible = false;
            textBox10.Visible = false;
            textBox11.Visible = false;
            textBox12.Visible = false;
            button6.Visible = false;
            button7.Visible = false;

            label1.Visible = true;
            label6.Visible = true;
            label8.Visible = true;
            textBox1.Visible = true;
            textBox6.Visible = true;
            checkBox2.Visible = true;
            button1.Visible = true;
            button2.Visible = true;
            button5.Visible = true;

            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();

            textBox1.Focus();
        }

        private void button7_MouseLeave(object sender, EventArgs e)
        {
            button7.BackColor = Color.FromArgb(76, 136, 74);
            button7.FlatAppearance.MouseOverBackColor = Color.FromArgb(76, 136, 74);
        }

        private void button7_MouseEnter(object sender, EventArgs e)
        {
            button7.BackColor = Color.FromArgb(51, 91, 49);
            button7.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 91, 49);
            
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox11.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox10.Focus();
                textBox10.SelectAll();
            }
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox10.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox12.Focus();
                textBox12.SelectAll();
            }
        }

        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox12.Text.Length >= 20 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                button7_Click(sender, e);
            }
        }
    }
}
