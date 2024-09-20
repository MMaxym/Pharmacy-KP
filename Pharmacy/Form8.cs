using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Newtonsoft.Json;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using DocumentFormat.OpenXml.Vml;

namespace Pharmacy
{
    public partial class Form8 : Form
    {
        private List<Users> users = new List<Users>();
        private const string jsonFileName = "Users.json";

        public Form8()
        {
            InitializeComponent();
            Users.LoadUsersFromJson();
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.WindowState = FormWindowState.Normal;
            this.MaximizeBox = false;

            this.Width = 470;
            this.Height = 780;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
            checkBox1.Checked = false;

            this.Close();
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
            else if (textBox8.Text.Length < 4)
            {
                MessageBox.Show("Пароль повинен бути не коротше 4 символів/цифр!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox8.Focus();
                return;
            }
            else if (textBox4.Text.Length < 10)
            {
                MessageBox.Show("Будь ласка, введіть номер телефону повністю!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Focus();
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
                textBox4.Focus();
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
                    IsAdministrator = checkBox1.Checked
                };

                Users.SaveUsersToJson(newUser);

                MessageBox.Show($"Акаунт для {newUser.Name} {newUser.Surname} створено успішно!", "Акаунт створено", MessageBoxButtons.OK, MessageBoxIcon.Information);

                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBox7.Clear();
                textBox8.Clear();
                textBox9.Clear();
                checkBox1.Checked = false;

                this.Close();
            }
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

        private void button3_MouseLeave(object sender, EventArgs e)
        { 
            button3.BackColor = Color.FromArgb(76, 136, 74);
            button3.FlatAppearance.MouseOverBackColor = Color.FromArgb(76, 136, 74);

        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(51, 91, 49);
            button3.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 91, 49);
        }
    }
}
