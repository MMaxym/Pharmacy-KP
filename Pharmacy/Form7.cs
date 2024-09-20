using ClosedXML.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Pharmacy
{
    public partial class Form7 : Form
    {
        public string jsonFileName = "Orders.json";
        private string backgroundText = "Введіть телефон для пошуку ...";
        private string backgroundText2 = "Введіть № замовлення для пошуку ...";
        public string UserName { get; set; }
        public int number { get; set; }

        public Form7()
        {
            InitializeComponent();
            FilrtView();
            dataGridView2.CellPainting += dataGridView2_CellPainting;
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = this.MaximumSize;
            this.MaximumSize = this.MaximumSize;
            this.MaximizeBox = false;

            dataGridView2.Columns["Column7"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            DisplayOrdersInDataGridView();

            textBox5.Text = backgroundText;
            textBox5.GotFocus += TextBox5_GotFocus;
            textBox5.LostFocus += TextBox5_LostFocus;

            textBox1.Text = backgroundText2;
            textBox1.GotFocus += TextBox1_GotFocus;
            textBox1.LostFocus += TextBox1_LostFocus;

            FilrtView();

            comboBox1.SelectedIndex = -1;
            dataGridView2.AllowUserToAddRows = false;

            this.Width = 1600;
            this.Height = 900;
            dataGridView2.Size = new Size(dataGridView2.Size.Height, 710);
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

        private void TextBox1_GotFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == backgroundText2)
            {
                textBox1.Text = "";
            }
        }

        private void TextBox1_LostFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                textBox1.Text = backgroundText2;
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

        private void DisplayOrdersInDataGridView()
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
                        dataGridView2.Rows.Add(order.Number, order.Name, order.PhoneNumber, order.DateTime, order.Status, order.Payment, order.Suma, GetProductsAsString(order.Products));
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (!dataGridView2.ReadOnly)
            {
                try
                {
                     DialogResult saveResult = MessageBox.Show("Бажаєте зберегти зміни перед виходом?", "Попередження", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                     if (saveResult == DialogResult.Yes)
                     {
                        SaveToFile();
                        dataGridView2.ReadOnly = true;
                     }
                     else if (saveResult == DialogResult.Cancel)
                     {
                         return;
                     }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при збереженні змін:\n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            FilrtView();
            DisplayOrdersInDataGridView();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilrtView();

            string selectedStatus = null;

            if (comboBox1.SelectedItem != null)
            {
                selectedStatus = comboBox1.SelectedItem.ToString();
            }

            FilterOrdersByStatus(selectedStatus);
        }

        private void FilterOrdersByStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                DisplayOrdersInDataGridView();
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
                        if (order.Status == status)
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

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView2.ReadOnly = false;
            dataGridView2.Columns["Column1"].ReadOnly = true;
            dataGridView2.Columns["Column2"].ReadOnly = true;
            dataGridView2.Columns["Column3"].ReadOnly = true;
            dataGridView2.Columns["Column4"].ReadOnly = true;
            dataGridView2.Columns["Column6"].ReadOnly = true;
            dataGridView2.Columns["Column7"].ReadOnly = true;
            dataGridView2.Columns["Column8"].ReadOnly = true;

            MessageBox.Show("Ви в режимі редагування !!!\n\nДля редагування доступний тільки стовбець 'Стасус замовлення' !", "Редагування", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            dataGridView2.ReadOnly = false;
            dataGridView2.Columns["Column1"].ReadOnly = true;
            dataGridView2.Columns["Column2"].ReadOnly = true;
            dataGridView2.Columns["Column3"].ReadOnly = true;
            dataGridView2.Columns["Column4"].ReadOnly = true;
            dataGridView2.Columns["Column6"].ReadOnly = true;
            dataGridView2.Columns["Column7"].ReadOnly = true;
            dataGridView2.Columns["Column8"].ReadOnly = true;

            MessageBox.Show("Ви в режимі редагування !!!\n\nДля редагування доступний тільки стовбець 'Стасус замовлення' !", "Редагування", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveToFile();
            dataGridView2.ReadOnly = true;
        }

        private void Form7_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!dataGridView2.ReadOnly)
            {
                try
                {
                    DialogResult saveResult = MessageBox.Show("Бажаєте зберегти зміни перед виходом?", "Попередження", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                    if (saveResult == DialogResult.Yes)
                    {
                        SaveToFile();
                        dataGridView2.ReadOnly = true;
                    }
                    else if (saveResult == DialogResult.Cancel)
                    {
                        e.Cancel = true; 
                        return;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при збереженні змін:\n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveToFile()
        {
            try
            {
                string jsonData = File.ReadAllText(jsonFileName);
                JArray ordersFromFile = JArray.Parse(jsonData);

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    int number = Convert.ToInt32(row.Cells["Column1"].Value);
                    string status = Convert.ToString(row.Cells["Column5"].Value);

                    JObject orderFromFile = ordersFromFile.FirstOrDefault(order => order["Number"].ToString() == number.ToString()) as JObject;

                    if (orderFromFile != null)
                    {
                        orderFromFile["Status"] = status;
                    }
                }

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    DateFormatString = "dd.MM.yyyy, HH:mm:ss",
                    Formatting = Formatting.Indented
                };

                File.WriteAllText(jsonFileName, JsonConvert.SerializeObject(ordersFromFile, settings));

                MessageBox.Show("Зміни збережено!", "Збереження", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при оновленні змін:\n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private void SearchOrdersByPhoneNumber(string searchText)
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
                        if (order.PhoneNumber.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            dataGridView2.Rows.Add(order.Number, order.Name, order.PhoneNumber, order.DateTime, order.Status, order.Payment, order.Suma, GetProductsAsString(order.Products));
                        }
                    }

                    if (dataGridView2.Rows.Count == 0)
                    {
                        MessageBox.Show("Замовлення не знайдені!", "Пошук", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox5.Text = backgroundText;
                        DisplayOrdersInDataGridView();
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

        private void SearchOrdersByOrderNumber(string searchText)
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
                        if (order.Number.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            dataGridView2.Rows.Add(order.Number, order.Name, order.PhoneNumber, order.DateTime, order.Status, order.Payment, order.Suma, GetProductsAsString(order.Products));
                        }
                    }

                    if (dataGridView2.Rows.Count == 0)
                    {
                        MessageBox.Show("Замовлення не знайдені!", "Пошук", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox1.Text = backgroundText2;
                        DisplayOrdersInDataGridView();
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

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Focused)
            {
                if (textBox5.Text != backgroundText)
                {
                    string searchQuery = textBox5.Text.Trim();
                    SearchOrdersByPhoneNumber(searchQuery);
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

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (textBox5.Text.Length >= 10 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Focused)
            {
                if (textBox1.Text != backgroundText2)
                {
                    string searchQuery = textBox1.Text.Trim();
                    SearchOrdersByOrderNumber(searchQuery);
                }
                else
                {
                    if (textBox1.Focused)
                    {
                        if (textBox1.Focused)
                        {
                            if (textBox1.Text == backgroundText2)
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
                    e.Graphics.DrawRectangle(p, new System.Drawing.Rectangle(e.CellBounds.Location, new Size(e.CellBounds.Width - 1, e.CellBounds.Height - 1)));
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

        private void ExportToExcel(string fileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Звіт 1");

                var titleRow = worksheet.Range("A1:H1");
                titleRow.Merge().Value = $"Звіт по всім замовленням №{number}";
                titleRow.Style.Font.Bold = true;
                titleRow.Style.Font.FontSize = 18;
                titleRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var headersStyle = worksheet.Range("A3:H3").Style;
                headersStyle.Font.Bold = true;
                headersStyle.Font.FontSize = 12;
                headersStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headersStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headersStyle.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headersStyle.Border.InsideBorder = XLBorderStyleValues.Thin;
                headersStyle.Border.OutsideBorderColor = XLColor.Black;
                headersStyle.Border.InsideBorderColor = XLColor.Black;
                headersStyle.Fill.BackgroundColor = XLColor.FromArgb(196, 231, 187);

                worksheet.Column("A").Width = 7; // Номер
                worksheet.Column("B").Width = 22; // ПІБ
                worksheet.Column("C").Width = 12; // Телефон
                worksheet.Column("D").Width = 20; // Дата
                worksheet.Column("E").Width = 12; // Статус
                worksheet.Column("F").Width = 15; // Спосіб оплати
                worksheet.Column("G").Width = 15; // До сплати
                worksheet.Column("H").Width = 80; // Замовлені товари

                for (int i = 0; i < dataGridView2.Columns.Count; i++)
                {
                    worksheet.Cell(3, i + 1).Value = dataGridView2.Columns[i].HeaderText;
                }

                var dataStyle = worksheet.Range("A3:H" + (dataGridView2.Rows.Count + 3)).Style;
                dataStyle.Font.FontSize = 10;
                dataStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                dataStyle.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataStyle.Border.InsideBorder = XLBorderStyleValues.Thin;
                dataStyle.Border.OutsideBorderColor = XLColor.Black;
                dataStyle.Border.InsideBorderColor = XLColor.Black;

                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView2.Columns.Count; j++)
                    {
                        object value = dataGridView2.Rows[i].Cells[j].Value;
                        if (value != null)
                        {
                            string cellValue = value.ToString();
                            if (j == 7 && cellValue.Length > 0)
                            {
                                worksheet.Cell(i + 4, j + 1).Value = cellValue;
                                worksheet.Cell(i + 4, j + 1).Style.Alignment.WrapText = true;
                                worksheet.Row(i + 4).Height = worksheet.Row(i + 4).Height + (cellValue.Length / 80) * 12;
                            }
                            else
                            {
                                worksheet.Cell(i + 4, j + 1).Value = cellValue;
                            }
                        }
                        else
                        {
                            worksheet.Cell(i + 4, j + 1).Value = string.Empty;
                        }
                    }
                }

                var infoRow = worksheet.Row(dataGridView2.Rows.Count + 8);
                infoRow.Cell("B").Value = "Звіт сформував:";
                infoRow.Cell("B").Style.Font.Bold = true;
                infoRow.Cell("C").Value = UserName;
                infoRow.Cell("C").Style.Font.Bold = true;
                infoRow.Cell("C").Style.Font.SetItalic();

                var infoRowData = worksheet.Row(dataGridView2.Rows.Count + 10);
                infoRowData.Cell("B").Value = $"Дата формування звіту:";
                infoRowData.Cell("B").Style.Font.Bold = true;
                infoRowData.Cell("C").Value = $"{System.DateTime.Now}";
                infoRowData.Cell("C").Style.Font.Bold = true;
                infoRowData.Cell("C").Style.Font.SetItalic();

                var signatureCell = worksheet.Cell(dataGridView2.Rows.Count + 8, 5);
                signatureCell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                var signatureRange2 = worksheet.Row(dataGridView2.Rows.Count + 9);
                signatureRange2.Cell("E").Value = "Підпис";
                signatureRange2.Cell("E").Style.Font.FontSize = 8;
                signatureRange2.Cell("E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                signatureRange2.Cell("E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                workbook.SaveAs(fileName);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            number = random.Next(100000, 999999);

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                string defaultFileName = $"Звіт по замовленням №{number}.xlsx";
                saveFileDialog.FileName = defaultFileName;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToExcel(saveFileDialog.FileName);
                    MessageBox.Show("Дані успішно експортовано до файлу Excel!", "Експорт у файл", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при експорті даних до файлу Excel: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

