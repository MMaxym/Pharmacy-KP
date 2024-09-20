using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Pharmacy
{
    public partial class Form4 : Form
    {

        private Form2 form2Instance;
        private List<Medicine> medicine;

        public Form4(Form2 form2, List<Medicine> medicineList)
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.WindowState = FormWindowState.Normal;
            this.MaximizeBox = false;

            form2Instance = form2;
            medicine = medicineList;

            dateTimePicker1.MinDate = DateTime.Today;
            numericUpDown1.Maximum = 100000;
            numericUpDown2.Maximum = 100000;

            this.Width = 475;
            this.Height = 800;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            checkBox1.Checked = false;
            numericUpDown1.Value = numericUpDown1.Minimum;
            numericUpDown2.Value = numericUpDown2.Minimum; 

            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(comboBox1.Text) || string.IsNullOrWhiteSpace(comboBox2.Text) || numericUpDown1.Value == 0 || numericUpDown2.Value == 0 || medicine == null)
            {
                MessageBox.Show("Заповніть всі поля!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Medicine newMedicine = new Medicine
                {
                    Name = textBox1.Text,
                    Category = comboBox1.Text,
                    Price = Convert.ToDouble(numericUpDown1.Value),
                    ExpiryDate = dateTimePicker1.Value.Date.ToString("dd.MM.yyyy"),
                    QuantityAvailable = Convert.ToInt32(numericUpDown2.Value),
                    Prescription = checkBox1.Checked,
                    Instruction = textBox3.Text,
                    Provider = comboBox2.Text
                };

                medicine.Add(newMedicine);

                form2Instance.SaveMedicineToJson();
                form2Instance.LoadDataToDataGridView();

                DialogResult result = MessageBox.Show($"Товар '{newMedicine.Name}' додано в базу успішно!!!\n\nБажаєте продовжити додавання товарів?", "Повідомлення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    textBox1.Clear();
                    textBox3.Clear();
                    comboBox1.SelectedIndex = -1;
                    comboBox2.SelectedIndex = -1;
                    checkBox1.Checked = false;
                    numericUpDown1.Value = numericUpDown1.Minimum;
                    numericUpDown2.Value = numericUpDown2.Minimum;
                }
                else
                {
                    textBox1.Clear();
                    textBox3.Clear();
                    comboBox1.SelectedIndex = -1;
                    comboBox2.SelectedIndex = -1;
                    checkBox1.Checked = false;
                    numericUpDown1.Value = numericUpDown1.Minimum;
                    numericUpDown2.Value = numericUpDown2.Minimum;
                    this.Close();
                }
            }
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (textBox1.Text.Length >= 5 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
        }

        private void numericUpDown2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (textBox1.Text.Length >= 5 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dateTimePicker1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (textBox1.Text.Length >= 30 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                comboBox1.Focus();
                comboBox1.SelectAll();
            }
        }

        private void numericUpDown1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (numericUpDown1.Text.Length >= 10 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            else if (!char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',' && (numericUpDown1.Text.Contains(",") || numericUpDown1.Text.Length == 0))
            {
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                dateTimePicker1.Focus();
                dateTimePicker1.Select();
            }
        }

        private void numericUpDown2_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (numericUpDown2.Text.Length >= 6 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                checkBox1.Focus();
                checkBox1.Select();
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox3.Text.Length >= 200 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                comboBox2.Focus();
                comboBox2.Select();
            }
        }

        private void dateTimePicker1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                numericUpDown2.Focus();
                numericUpDown2.Select();
            }
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                numericUpDown1.Focus();
                numericUpDown1.Select();
            }
        }

        private void checkBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox3.Focus();
                textBox3.SelectAll();
            }
        }

        private void comboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                button3_Click(sender, e);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

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
