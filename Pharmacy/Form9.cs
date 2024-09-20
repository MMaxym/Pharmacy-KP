using DocumentFormat.OpenXml.Drawing.Charts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Pharmacy
{
    public partial class Form9 : Form
    {
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        private const string jsonFileName = "Orders.json";
        private string backgroundText = "Введіть № замовлення для пошуку ...";

        public Form9()
        {
            InitializeComponent();
        }

        private void Form9_Load(object sender, EventArgs e)
        {
            dataGridView2.Size = new System.Drawing.Size(this.Width, 300);

            this.MaximizeBox = false;
            this.Width = 1100;
            this.Height = 600;

            comboBox1.SelectedIndex = -1;
            FilrtView();

            dataGridView2.Columns["Column7"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView2.AllowUserToAddRows = false;

            List<Orders> orders2 = LoadOrdersFromFile(jsonFileName);
           
            foreach (var order in orders2)
            {
                if (order.Name != UserName && order.PhoneNumber != UserPhone)
                {
                    dataGridView2.Visible = false;
                    pictureBox1.Visible = false;
                    pictureBox4.Visible = false;
                    pictureBox5.Visible = false;
                    textBox1.Visible = false;
                    comboBox1.Visible = false;
                    label2.Visible = false;
                    button4.Visible = false;
                    label3.Visible = true;
                }
                else
                {
                    dataGridView2.Visible = true;
                    pictureBox1.Visible = true;
                    pictureBox4.Visible = true;
                    pictureBox5.Visible = true;
                    textBox1.Visible = true;
                    comboBox1.Visible = true;
                    label2.Visible = true;
                    button4.Visible = true;
                    label3.Visible = false;

                    DisplayOrdersInDataGridView(UserName, UserPhone);
                }

            }

            textBox1.Text = backgroundText;
            textBox1.GotFocus += TextBox1_GotFocus;
            textBox1.LostFocus += TextBox1_LostFocus;

            dataGridView2.Size = new System.Drawing.Size(dataGridView2.Size.Height, 470);
        }

        private List<Orders> LoadOrdersFromFile(string jsonFileName)
        {
            List<Orders> orders = null;
            try
            {
                if (File.Exists(jsonFileName))
                {
                    string jsonData = File.ReadAllText(jsonFileName);
                    orders = JsonConvert.DeserializeObject<List<Orders>>(jsonData);
                }
                else
                {
                    MessageBox.Show("Файл Orders.json не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return orders;
        }

        private void FilrtView()
        {
            if (comboBox1.SelectedItem == null)
            {
                label2.Visible = true;
            }
            else
            {
                label2.Visible = false;
            }
        }

        private void DisplayOrdersInDataGridView(string userName, string userPhone)
        {
            try
            {
                if (File.Exists(jsonFileName))
                {
                    string jsonData = File.ReadAllText(jsonFileName);
                    List<Orders> orders = JsonConvert.DeserializeObject<List<Orders>>(jsonData);

                    dataGridView2.Rows.Clear();

                    foreach (var order in orders)
                    {
                        if (order.Name == userName && order.PhoneNumber == userPhone)
                        {
                            dataGridView2.Rows.Add(order.Number, order.Name, order.PhoneNumber, order.DateTime, order.Status, order.Payment, order.Suma, GetProductsAsString(order.Products));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Файл Orders.json не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterOrdersByStatus(string status, string userName, string userPhone)
        {
            if (string.IsNullOrEmpty(status))
            {
                DisplayOrdersInDataGridView(userName, userPhone);
                return;
            }

            string jsonFileName = "Orders.json";

            try
            {
                if (File.Exists(jsonFileName))
                {
                    string jsonData = File.ReadAllText(jsonFileName);
                    List<Orders> orders = JsonConvert.DeserializeObject<List<Orders>>(jsonData);

                    dataGridView2.Rows.Clear();

                    foreach (var order in orders)
                    {
                        if (order.Status == status && order.Name == userName && order.PhoneNumber == userPhone)
                        {
                            dataGridView2.Rows.Add(order.Number, order.Name, order.PhoneNumber, order.DateTime, order.Status, order.Payment, order.Suma, GetProductsAsString(order.Products));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Файл Orders.json не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetProductsAsString(List<ProductsInOrders> products)
        {
            if (products == null || products.Count == 0)
            {
                return string.Empty;
            }

            List<string> productDetails = new List<string>();

            foreach (var product in products)
            {
                productDetails.Add($"Товар: '{product.Name}'  -  Кількість: {product.Quantity}");
            }

            return string.Join(",   ", productDetails);
        }

        private void SearchOrdersByOrderNumber(string searchText, string userName, string userPhone)
        {
            try
            {
                if (File.Exists(jsonFileName))
                {
                    string jsonData = File.ReadAllText(jsonFileName);
                    List<Orders> orders = JsonConvert.DeserializeObject<List<Orders>>(jsonData);

                    dataGridView2.Rows.Clear();

                    foreach (var order in orders)
                    {
                        if (order.Number.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 &&
                            order.Name == userName && order.PhoneNumber == userPhone)
                        {
                            dataGridView2.Rows.Add(order.Number, order.Name, order.PhoneNumber, order.DateTime, order.Status, order.Payment, order.Suma, GetProductsAsString(order.Products));
                        }
                    }

                    if (dataGridView2.Rows.Count == 0)
                    {
                        MessageBox.Show("Замовлення не знайдені!", "Пошук", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox1.Text = backgroundText;
                        DisplayOrdersInDataGridView(userName, userPhone);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Файл Orders.json не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            FilrtView();
            DisplayOrdersInDataGridView(UserName, UserPhone);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Focused)
            {
                if (textBox1.Text != backgroundText)
                {
                    string searchQuery = textBox1.Text.Trim();
                    SearchOrdersByOrderNumber(searchQuery, UserName, UserPhone);
                }
                else
                {
                    if (textBox1.Focused)
                    {
                        if (textBox1.Focused)
                        {
                            if (textBox1.Text == backgroundText)
                            {
                                textBox1.Text = "";
                            }
                        }
                    }
                    return;
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (textBox1.Text.Length >= 6 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilrtView();

            string selectedStatus = null;

            if (comboBox1.SelectedItem != null)
            {
                selectedStatus = comboBox1.SelectedItem.ToString();
            }

            FilterOrdersByStatus(selectedStatus, UserName, UserPhone);
        }

        private void TextBox1_GotFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == backgroundText)
            {
                textBox1.Text = "";
            }
        }

        private void TextBox1_LostFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                textBox1.Text = backgroundText;
            }
        }

        private void dataGridView2_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }

            if ((e.RowIndex == dataGridView2.CurrentCell.RowIndex && e.RowIndex >= 0 && e.ColumnIndex >= 0) || e.RowIndex == -1)
            {
                e.PaintBackground(e.ClipBounds, true);

                if (e.RowIndex == dataGridView2.CurrentCell.RowIndex && e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    Color darkGreen = Color.FromArgb(76, 136, 74);
                    e.Graphics.FillRectangle(new SolidBrush(darkGreen), e.CellBounds);
                }
                else
                {
                    e.Graphics.FillRectangle(Brushes.Silver, e.CellBounds);
                }

                using (Pen p = new Pen(Color.Black))
                {
                    e.Graphics.DrawRectangle(p, new Rectangle(e.CellBounds.Location, new System.Drawing.Size(e.CellBounds.Width - 1, e.CellBounds.Height - 1)));
                }

                if (e.Value != null)
                {
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    Brush textColor = (e.RowIndex == dataGridView2.CurrentCell.RowIndex && e.RowIndex >= 0 && e.ColumnIndex >= 0) ? Brushes.White : Brushes.Black;

                    e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font, textColor, e.CellBounds, sf);
                }

                e.Handled = true;
            }
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Gainsboro;
            }
            else
            {
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(196, 231, 187);
            }
        }
    }
}
