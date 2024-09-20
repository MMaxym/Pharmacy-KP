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
using Newtonsoft.Json.Linq;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Taskbar;
using System.Text.RegularExpressions;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Reflection.Emit;

namespace Pharmacy
{
    public partial class Form3 : Form
    {
        private List<Medicine> medicine = new List<Medicine>();
        private const string jsonFileName = "Medicine.json";
        private string backgroundText = "Введіть назву для пошуку ...";

        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string UserLogin { get; set; }
        public ShoppingCart cart;

        Form1 form1 = new Form1();

        private void Form3_Load(object sender, EventArgs e)
        {
            label10.Text = UserName;
            dataGridView2.Size = new Size(dataGridView2.Size.Height, 700);
        }

        public Form3()
        {
            InitializeComponent();
            LoadMedicineFromJson();

            this.MaximizeBox = false;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = this.MaximumSize;
            this.MaximumSize = this.MaximumSize;

            dataGridView2.Columns["dataGridViewTextBoxColumn8"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView2.Columns["dataGridViewTextBoxColumn1"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView2.Columns["dataGridViewTextBoxColumn9"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            FilrtView();

            DisplayMedicineInDataGridView();

            comboBox1.SelectedIndex = -1;

            textBox5.Text = backgroundText;
            textBox5.GotFocus += TextBox5_GotFocus;
            textBox5.LostFocus += TextBox5_LostFocus;

            FilrtView();

            dataGridView2.AutoGenerateColumns = true;
            dataGridView2.AllowUserToAddRows = false;

            cart = new ShoppingCart(UserName, UserPhone, new List<ProductsInOrders>());

            dataGridView2.CellPainting += dataGridView2_CellPainting;

        }
        
        private void TextBox5_GotFocus(object sender, EventArgs e)
        {
            if (textBox5.Text == backgroundText)
            {
                textBox5.Text = "";
            }
        }

        private void TextBox5_LostFocus(object sender, EventArgs e)
        {
            if (textBox5.Text == string.Empty)
            {
                textBox5.Text = backgroundText;
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Focused)
            {
                if (textBox5.Text != backgroundText)
                {
                    LoadMedicineFromJson();
                    DisplayMedicineInDataGridView();
                    string searchQuery = textBox5.Text.Trim();
                    PerformSearch(searchQuery);
                }
                else
                {
                    if (textBox5.Focused)
                    {
                        if (textBox5.Focused)
                        {
                            if (textBox5.Text == backgroundText)
                            {
                                textBox5.Text = "";
                            }
                        }
                    }
                    return;
                }
            }
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

        public void LoadMedicineFromJson()
        {
            try
            {
                if (File.Exists(jsonFileName))
                {
                    string json = File.ReadAllText(jsonFileName, Encoding.UTF8);
                    var medicinesWithoutProvider = JsonConvert.DeserializeObject<List<Medicine>>(json);

                    medicine = medicinesWithoutProvider;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні користувачів: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PerformSearch(string searchQuery)
        {
            string jsonData = File.ReadAllText(jsonFileName);
            JArray medicines = JArray.Parse(jsonData);
            dataGridView2.Rows.Clear();
            bool found = false;

            foreach (var med in medicines)
            {
                string name = med["Name"].ToString();
                string category = med["Category"].ToString();
                string price = med["Price"].ToString();
                string expiryDate = med["ExpiryDate"].ToString();
                string quantityAvailable = med["QuantityAvailable"].ToString();
                bool prescription = med["Prescription"].ToObject<bool>();
                string instruction = med["Instruction"].ToString();
                string provider = med["Provider"].ToString();

                if (name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string prescriptionValue = prescription ? "Так" : "Ні";
                    dataGridView2.Rows.Add(name, category, price, expiryDate, quantityAvailable, prescriptionValue, instruction, provider);
                    found = true;
                }
            }
            if (!found)
            {
                MessageBox.Show("Товар не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DisplayMedicineInDataGridView();
                textBox5.Text = backgroundText;
                return;
            }
        }

        public void LoadDataToDataGridView()
        {
            dataGridView2.Rows.Clear();

            LoadMedicineFromJson();

            DisplayMedicineInDataGridView();
        }

        private void DisplayMedicineInDataGridView()
        {
            dataGridView2.Rows.Clear();

            foreach (var med in medicine)
            {
                string prescriptionValue = med.Prescription ? "Так" : "Ні";
                dataGridView2.Rows.Add(med.Name, med.Category, med.Price, med.ExpiryDate, med.QuantityAvailable, prescriptionValue, med.Instruction, med.Provider);
            }
        }

        private bool IsDataEqual(JArray medicinesFromFile)
        {
            if (dataGridView2.Rows.Count != medicinesFromFile.Count)
            {
                return false;
            }

            for (int i = 0; i < medicinesFromFile.Count; i++)
            {
                var medicineFromFile = (JObject)medicinesFromFile[i];

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells["dataGridViewTextBoxColumn1"].Value.ToString() != medicineFromFile["Name"].ToString() ||
                        row.Cells["dataGridViewTextBoxColumn2"].Value.ToString() != medicineFromFile["Category"].ToString() ||
                        row.Cells["dataGridViewTextBoxColumn3"].Value.ToString() != medicineFromFile["Price"].ToString() ||
                        row.Cells["dataGridViewTextBoxColumn4"].Value.ToString() != medicineFromFile["ExpiryDate"].ToString() ||
                        row.Cells["dataGridViewTextBoxColumn6"].Value.ToString() != medicineFromFile["QuantityAvailable"].ToString() ||
                        row.Cells["dataGridViewTextBoxColumn7"].Value.ToString() != medicineFromFile["Prescription"].ToString() ||
                        row.Cells["dataGridViewTextBoxColumn8"].Value.ToString() != medicineFromFile["Instruction"].ToString() ||
                        row.Cells["dataGridViewTextBoxColumn9"].Value.ToString() != medicineFromFile["Provider"].ToString()
                        )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView2.Columns["dataGridViewTextBoxColumn10"].Index && e.RowIndex >= 0)
            {
                string column1Value = dataGridView2.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn1"].Value.ToString();
                double column3Value = Convert.ToDouble(dataGridView2.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn3"].Value.ToString());
                int column6Value = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn6"].Value.ToString());

                Form5 form5 = new Form5();

                form5.Name = column1Value;
                form5.Price = column3Value;
                form5.Quantity = column6Value;
                form5.Cart = cart;

                form5.ShowDialog();
            }

        }

        private void Form3_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void DisplayMedicineByCategory(string category)
        {
            dataGridView2.Rows.Clear();

            foreach (var med in medicine)
            {
                if (med.Category == category)
                {
                    string prescriptionValue = med.Prescription ? "Так" : "Ні";
                    dataGridView2.Rows.Add(med.Name, med.Category, med.Price, med.ExpiryDate, med.QuantityAvailable, prescriptionValue, med.Instruction, med.Provider);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form9 form9 = new Form9();
            form9.UserName = UserName;
            form9.UserPhone = UserPhone;
            form9.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();
            form6.UserName = label10.Text;
            form6.UserPhone = UserPhone;
            form6.Cart = cart;
            form6.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            form1.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            FilrtView();
            DisplayMedicineInDataGridView();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilrtView();
            if (comboBox1.SelectedItem != null)
            {
                string selectedCategory = comboBox1.SelectedItem.ToString();
                DisplayMedicineByCategory(selectedCategory);
            }
            else
            {
                DisplayMedicineInDataGridView();
            }
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
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
                    e.Graphics.DrawRectangle(p, new Rectangle(e.CellBounds.Location, new Size(e.CellBounds.Width - 1, e.CellBounds.Height - 1)));
                }

                if (e.Value != null)
                {
                    StringFormat sf = new StringFormat();
                    if (e.ColumnIndex == dataGridView2.Columns.Count - 1)
                    {
                        sf.Alignment = StringAlignment.Center;
                    }
                    else
                    {
                        sf.Alignment = StringAlignment.Near;
                    }

                    Brush textColor;
                    if (e.RowIndex == dataGridView2.CurrentCell.RowIndex && e.RowIndex >= 0 && e.ColumnIndex >= 0)
                    {
                        textColor = Brushes.White;
                    }
                    else
                    {
                        textColor = Brushes.Black;
                    }

                    e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font, textColor, e.CellBounds, sf);
                }

                e.Handled = true;
            }
        }
    }
}
