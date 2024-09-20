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
using ClosedXML.Excel;
using static ClosedXML.Excel.XLPredefinedFormat;

namespace Pharmacy
{
    public partial class Form2 : Form
    {
        private List<Medicine> medicine = new List<Medicine>();
        private const string jsonFileName = "Medicine.json";
        private string backgroundText = "Введіть назву для пошуку ...";
        public string UserName {  get; set; }
        public int number {  get; set; }

        Form1 form1 = new Form1();

        public Form2()
        {
            InitializeComponent();
            LoadMedicineFromJson();

            comboBox1.SelectedIndex = -1;

            FilrtView();

            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = this.MaximumSize;
            this.MaximumSize = this.MaximumSize;

            dataGridView2.Columns["dataGridViewTextBoxColumn8"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView2.Columns["dataGridViewTextBoxColumn1"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView2.Columns["dataGridViewTextBoxColumn9"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            DisplayMedicineInDataGridView();
                
            textBox5.Text = backgroundText;
            textBox5.GotFocus += TextBox5_GotFocus;
            textBox5.LostFocus += TextBox5_LostFocus;

            dataGridView2.AutoGenerateColumns = true;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.CellPainting += dataGridView2_CellPainting;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label10.Text = UserName;
            FilrtView();
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

        public void SaveMedicineToJson()
        {
            try
            {
                string json = JsonConvert.SerializeObject(medicine);
                File.WriteAllText(jsonFileName, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMedicineFromJson()
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

        public void LoadDataToDataGridView()
        {
            dataGridView2.Rows.Clear();

            LoadMedicineFromJson();

            DisplayMedicineInDataGridView();
        }

        private void DisplayMedicineInDataGridView()
        {
            dataGridView2.Rows.Clear();

            medicine.Sort((x, y) => x.Name.CompareTo(y.Name));

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

        private void UpdateDataInFile(string jsonFileName)
        {
            JArray updatedMedicines = new JArray();

            string jsonData = File.ReadAllText(jsonFileName);
            JArray medicinesFromFile = JArray.Parse(jsonData);

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                string name = row.Cells["dataGridViewTextBoxColumn1"].Value?.ToString();
                string category = row.Cells["dataGridViewTextBoxColumn2"].Value?.ToString();
                string price = row.Cells["dataGridViewTextBoxColumn3"].Value?.ToString();
                string expiryDate = row.Cells["dataGridViewTextBoxColumn4"].Value?.ToString();
                string quantityAvailable = row.Cells["dataGridViewTextBoxColumn6"].Value?.ToString();
                string prescriptionString = row.Cells["dataGridViewTextBoxColumn7"].Value?.ToString();
                string instruction = row.Cells["dataGridViewTextBoxColumn8"].Value?.ToString();
                string providerName = row.Cells["dataGridViewTextBoxColumn9"].Value?.ToString();

                JObject medicineFromFile = medicinesFromFile.FirstOrDefault(m => m["Name"].ToString() == name) as JObject;

                if (medicineFromFile != null)
                {
                    if (category != null)
                        medicineFromFile["Category"] = category;
                    if (!string.IsNullOrWhiteSpace(price))
                    {
                        if (double.TryParse(price, out double priceValue))
                        {
                            medicineFromFile["Price"] = priceValue;
                        }
                        else
                        {
                            throw new Exception($"Недопустиме значення '{price}' для поля Ціна\n\nВведіть числове значення!");
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(quantityAvailable))
                    {
                        if (int.TryParse(quantityAvailable, out int quantityValue))
                        {
                            medicineFromFile["QuantityAvailable"] = quantityValue;
                        }
                        else
                        {
                            throw new Exception($"Недопустиме значення '{quantityAvailable}' для поля Кількість в наявності\n\nВведіть ціле числове значення!");
                        }
                    }
                    if (expiryDate != null)
                        medicineFromFile["ExpiryDate"] = expiryDate;

                    if (!string.IsNullOrEmpty(prescriptionString))
                    {
                        bool prescriptionValue;
                        if (prescriptionString.Equals("Так", StringComparison.OrdinalIgnoreCase))
                        {
                            prescriptionValue = true;
                        }
                        else if (prescriptionString.Equals("Ні", StringComparison.OrdinalIgnoreCase))
                        {
                            prescriptionValue = false;
                        }
                        else
                        {
                            throw new Exception($"Недопустиме значення '{prescriptionString}' для поля З рецептом\n\nМожливе значення - Так/Ні");
                        }

                        medicineFromFile["Prescription"] = prescriptionValue;
                    }
                    if (instruction != null)
                        medicineFromFile["Instruction"] = instruction;

                    medicineFromFile["Provider"] = providerName;
                }
            }

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd",
                Formatting = Formatting.Indented
            };
            File.WriteAllText(jsonFileName, JsonConvert.SerializeObject(medicinesFromFile, settings));

            MessageBox.Show("Зміни збережено!", "Збереження", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void SaveAndLoadDataToDataGridView()
        {
            SaveMedicineToJson();
            LoadDataToDataGridView();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4(this, medicine);
            form4.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView2.ReadOnly = false;

            dataGridView2.Columns["dataGridViewTextBoxColumn4"].ReadOnly = true;

            MessageBox.Show("Ви в режимі редагування!!!", "Успішно", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bool hasEmptyCells = CheckForEmptyCells();
            if (hasEmptyCells)
            {
                MessageBox.Show("Наявні порожні комірки у таблиці!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string jsonData = File.ReadAllText(jsonFileName);
            JArray medicinesFromFile = JArray.Parse(jsonData);

            try
            {
                if (IsDataEqual(medicinesFromFile))
                {
                    MessageBox.Show("Дані не змінилися!", "Збереження", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                   UpdateDataInFile(jsonFileName);
                }
                dataGridView2.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні змін:\n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private bool CheckForEmptyCells()
        {
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        return true; 
                    }
                }
            }
            return false; 
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            form1.ShowDialog();
        }

        private void dataGridView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            dataGridView2.ReadOnly = false;

            dataGridView2.Columns["dataGridViewTextBoxColumn4"].ReadOnly = true;

            MessageBox.Show("Ви у режимі редагування!!!", "Редагування", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedCells.Count > 0)
            {
                int selectedRowIndex = dataGridView2.SelectedCells[0].RowIndex;
                string productName = dataGridView2.Rows[selectedRowIndex].Cells["dataGridViewTextBoxColumn1"].Value.ToString();

                DialogResult result = MessageBox.Show($"Ви дійсно бажаєте видалити товар '{productName}'?", "Підтвердження видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dataGridView2.Rows.RemoveAt(selectedRowIndex);

                    medicine.RemoveAt(selectedRowIndex);

                    SaveMedicineToJson();

                    MessageBox.Show($"Товар '{productName}' успішно видалено", "Видалення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Спочатку виділіть рядок, який бажаєте видалити.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!dataGridView2.ReadOnly)
            {
                bool hasEmptyCells = CheckForEmptyCells();
                if (hasEmptyCells)
                {
                    MessageBox.Show("Наявні порожні комірки у таблиці!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }

                string jsonData = File.ReadAllText(jsonFileName);
                JArray medicinesFromFile = JArray.Parse(jsonData);

                bool dataChanged = !IsDataEqual(medicinesFromFile);

                if (dataChanged)
                {
                    DialogResult saveResult = MessageBox.Show("Бажаєте зберегти зміни перед виходом?", "Попередження", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                    if (saveResult == DialogResult.Yes)
                    {
                        try
                        {
                            UpdateDataInFile(jsonFileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Помилка при збереженні:\n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true; 
                            return;
                        }
                    }
                    else if (saveResult == DialogResult.Cancel)
                    {
                        e.Cancel = true; 
                        return;
                    }
                }
            }
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
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

        private void FilrtView()
        {
            if(comboBox1.SelectedItem == null)
            {
                label1.Visible = true;
            }
            else
            {
                label1.Visible = false;
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

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            FilrtView();
            DisplayMedicineInDataGridView();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.UserName = UserName;
            form7.ShowDialog();
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

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
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
                    sf.LineAlignment = StringAlignment.Center;
                    Brush textColor = (e.RowIndex == dataGridView2.CurrentCell.RowIndex && e.RowIndex >= 0 && e.ColumnIndex >= 0) ? Brushes.White : Brushes.Black;

                    e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font, textColor, e.CellBounds, sf);
                }

                e.Handled = true;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form8 form8 = new Form8();
            form8.ShowDialog();
        }

        private void ExportToExcel(string fileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Звіт 1");

                var titleRow = worksheet.Range("A1:H1");
                titleRow.Merge().Value = $"Звіт по всім товарам №{number}";
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

                worksheet.Column("A").Width = 30; // Назва
                worksheet.Column("B").Width = 25; // Категорія
                worksheet.Column("C").Width = 22; // Ціна за одиницю
                worksheet.Column("D").Width = 22; // Строк придатності
                worksheet.Column("E").Width = 22; // Кількість в наявності
                worksheet.Column("F").Width = 15; // Рецепт
                worksheet.Column("G").Width = 60; // Спосіб застосування
                worksheet.Column("H").Width = 18; // Постачальник

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
                    bool hasData = false;
                    for (int j = 0; j < dataGridView2.Columns.Count; j++)
                    {
                        object value = dataGridView2.Rows[i].Cells[j].Value;
                        worksheet.Cell(i + 4, j + 1).Value = value != null ? value.ToString() : string.Empty;
                        if (value != null && !string.IsNullOrEmpty(value.ToString()))
                        {
                            hasData = true;
                        }
                    }
                }

                var infoRow = worksheet.Row(dataGridView2.Rows.Count + 8);
                infoRow.Cell("A").Value = "Звіт сформував адміністратор:";
                infoRow.Cell("A").Style.Font.Bold = true;
                infoRow.Cell("B").Value = UserName; 
                infoRow.Cell("B").Style.Font.Bold = true;
                infoRow.Cell("B").Style.Font.SetItalic();

                var infoRowData = worksheet.Row(dataGridView2.Rows.Count + 10);
                infoRowData.Cell("A").Value = $"Дата формування звіту:";
                infoRowData.Cell("A").Style.Font.Bold = true;
                infoRowData.Cell("B").Value = $"{System.DateTime.Now}";
                infoRowData.Cell("B").Style.Font.Bold = true;
                infoRowData.Cell("B").Style.Font.SetItalic();

                var signatureCell = worksheet.Cell(dataGridView2.Rows.Count + 8, 3);
                signatureCell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                var signatureRange2 = worksheet.Row(dataGridView2.Rows.Count + 9);
                signatureRange2.Cell("C").Value = "Підпис";
                signatureRange2.Cell("C").Style.Font.FontSize = 8;
                signatureRange2.Cell("C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                signatureRange2.Cell("C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

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

                string defaultFileName = $"Звіт по товарам №{number}.xlsx";
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