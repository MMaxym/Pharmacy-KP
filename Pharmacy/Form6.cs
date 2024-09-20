using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Pharmacy
{
    public partial class Form6 : Form
    {
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public ShoppingCart Cart { get; set; }
        public int orderNumber { get; set; }

        private const string jsonFileName = "Orders.json";

        public Form6()
        {
            InitializeComponent();
            dataGridView2.CellPainting += dataGridView2_CellPainting;

            dataGridView2.Columns["dataGridViewTextBoxColumn1"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;

            label1.Text = UserName;
            label2.Text = UserPhone;

            comboBox1.SelectedIndex = -1;

            if (Cart != null && Cart.ProductsInOrders.Count == 0)
            {
                dataGridView2.Visible = false;
                label3.Visible = true;
                label5.Text = "0.00 грн";
            }
            else
            {
                PopulateDataGridView(Cart.ProductsInOrders);
            }

            dataGridView2.AllowUserToAddRows = false;
            
        }

        public void PopulateDataGridView(List<ProductsInOrders> products)
        {
            dataGridView2.Rows.Clear();

             double totalCost = 0;

            if (products != null && products.Count != 0)
            {
                foreach (var product in products)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataGridView2);

                    row.Cells[dataGridView2.Columns["dataGridViewTextBoxColumn1"].Index].Value = product.Name;
                    row.Cells[dataGridView2.Columns["dataGridViewTextBoxColumn3"].Index].Value = product.Price;
                    row.Cells[dataGridView2.Columns["dataGridViewTextBoxColumn11"].Index].Value = product.Quantity;

                    dataGridView2.Rows.Add(row);

                    double productCost = product.Price * product.Quantity;
                    totalCost += productCost;
                }
            }
            else
            {
                dataGridView2.Visible = false;
                label3.Visible = true;
            }

            label5.Text = totalCost.ToString("0.00") + " грн";
        }

        private void RemoveRow(int index)
        {
            if (index >= 0 && index < dataGridView2.Rows.Count)
            {
                dataGridView2.Rows.RemoveAt(index);

                if (dataGridView2.Rows.Count == 0)
                {
                    dataGridView2.Visible = false;
                    label3.Visible = true;
                }
            }
        }

        private void GeneratePDFReceipt(string filePath)
        { 
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (iTextSharp.text.Document document = new iTextSharp.text.Document())
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(document, fs);

                        document.Open();
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                        BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                        Paragraph header = new Paragraph("ЗАМОВЛЕННЯ\n\n", new iTextSharp.text.Font(baseFont, 20f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        header.Alignment = Element.ALIGN_CENTER;
                        document.Add(header);

                        Paragraph orderInfo = new Paragraph();
                        orderInfo.Add(new iTextSharp.text.Chunk($"НОМЕР ЗАМОВЛЕННЯ:  {orderNumber}\n\n", new iTextSharp.text.Font(baseFont, 12f)));
                        orderInfo.Add(new iTextSharp.text.Chunk($"КЛІЄНТ:  {UserName}\n\n", new iTextSharp.text.Font(baseFont, 12f))); ;
                        orderInfo.Add(new iTextSharp.text.Chunk($"ТЕЛЕФОН:  {UserPhone}\n\n", new iTextSharp.text.Font(baseFont, 12f)));
                        orderInfo.Add(new iTextSharp.text.Chunk($"ДАТА ОФОРМЛЕННЯ:  {DateTime.Now.ToString("dd.MM.yyyy, HH:mm:ss")}\n", new iTextSharp.text.Font(baseFont, 12f)));
                        orderInfo.Alignment = Element.ALIGN_LEFT;
                        document.Add(orderInfo);

                        document.Add(new iTextSharp.text.Chunk("\n----------------------------------------------------------------------------------------------------------------------------------\n\n"));

                        Paragraph productsHeader = new Paragraph("Товари в замовленні:\n\n", new iTextSharp.text.Font(baseFont, 14f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        document.Add(productsHeader);

                        StringBuilder productListBuilder = new StringBuilder();

                        foreach (var product in Cart.ProductsInOrders)
                        {
                            string productName = product.Name;
                            double price = product.Price;
                            int quantity = product.Quantity;


                            string formattedProduct = String.Format($"{productName}   -   {price} грн   -   к-сть: {quantity}");
                            productListBuilder.AppendLine(formattedProduct);
                        }

                        string productList = productListBuilder.ToString();

                        Paragraph productsList = new Paragraph(productList, new iTextSharp.text.Font(baseFont, 12f));
                        document.Add(productsList);

                        document.Add(new iTextSharp.text.Chunk("\n----------------------------------------------------------------------------------------------------------------------------------\n\n"));

                        string totalCost = label5.Text.ToString();
                        Paragraph totalAmount = new Paragraph($"Загальна сума замовлення: {totalCost}", new iTextSharp.text.Font(baseFont, 12f));
                        document.Add(totalAmount);

                        document.Add(new iTextSharp.text.Chunk("\n----------------------------------------------------------------------------------------------------------------------------------\n\n"));

                        Paragraph congratsMessage = new Paragraph("Вітаємо з покупкою !!!\nБажаємо швидкого одуження !!!", new iTextSharp.text.Font(baseFont, 14f, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        congratsMessage.Alignment = Element.ALIGN_CENTER;
                        document.Add(congratsMessage);

                        document.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при створенні PDF: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void UpdateMedicineQuantity(string productName, int quantity)
        {
            try
            {
                string json = File.ReadAllText("Medicine.json");
                List<Medicine> medicineList = JsonConvert.DeserializeObject<List<Medicine>>(json);

                foreach (Medicine product in medicineList)
                {
                    if (product.Name == productName)
                    {
                        product.QuantityAvailable -= quantity;
                        break;
                    }
                }

                string updatedJson = JsonConvert.SerializeObject(medicineList);
                File.WriteAllText("Medicine.json", updatedJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при оновленні кількості товару: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                List<ProductsInOrders> productsInOrder = Cart.ProductsInOrders;
                Random random = new Random();
                orderNumber = random.Next(100000, 999999);

                if (productsInOrder.Count == 0)
                {
                    MessageBox.Show("Ваш кошик порожній !\nДодайте товари в замовлення !", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Оберіть спосіб оплати!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (ProductsInOrders product in productsInOrder)
                {
                    UpdateMedicineQuantity(product.Name, product.Quantity);
                }

                MessageBox.Show("Ваше замовлення успішно створено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DateTime orderDateTime = DateTime.Now;
                Orders order = new Orders(orderNumber, UserName, UserPhone, orderDateTime.ToString(), "В обробці", comboBox1.SelectedItem.ToString(), label5.Text.ToString(), productsInOrder);
                Orders.SaveOrderToJson(order, jsonFileName);

                DialogResult result = MessageBox.Show("Бажаєте завантажити файл з товарами у замовленні?", "Замовлення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                        saveFileDialog.Title = "Зберегти замовлення";
                        saveFileDialog.FileName = $"Замовлення_№{orderNumber}.pdf";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string filePath = saveFileDialog.FileName;

                            GeneratePDFReceipt(filePath);

                            MessageBox.Show($"Замовлення збережено у файлі: {filePath}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при генерації файлу замовлення: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                dataGridView2.Rows.Clear();
                dataGridView2.Visible = false;
                label3.Visible = true;
                Cart.ProductsInOrders.Clear();
                label5.Text = "0.00 грн";

                Form3 form3 = Application.OpenForms.OfType<Form3>().FirstOrDefault();
                if (form3 != null)
                {
                    form3.LoadDataToDataGridView();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при оформленні замовлення: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView2.Columns["dataGridViewTextBoxColumn13"].Index && e.RowIndex >= 0)
            {
                if (Cart != null && Cart.ProductsInOrders != null && Cart.ProductsInOrders.Count > e.RowIndex)
                {
                    ProductsInOrders product = Cart.ProductsInOrders[e.RowIndex];

                    DialogResult result = MessageBox.Show("Ви дійсно бажаєте видалити з кошика товар '" + product.Name + "'?", "Підтвердження видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Cart.ProductsInOrders.RemoveAt(e.RowIndex);

                        RemoveRow(e.RowIndex);

                        double currentTotal = double.Parse(label5.Text.Split(' ')[0]);

                        double removedProductCost = product.Price * product.Quantity;
                        double newTotal = currentTotal - removedProductCost;

                        label5.Text = newTotal.ToString("0.00") + " грн";
                    }
                }
            }

            if (e.ColumnIndex == dataGridView2.Columns["dataGridViewTextBoxColumn12"].Index && e.RowIndex >= 0)
            {
                if (Cart != null && Cart.ProductsInOrders != null && Cart.ProductsInOrders.Count > e.RowIndex)
                {
                    ProductsInOrders product = Cart.ProductsInOrders[e.RowIndex];
                    
                    product.Quantity++;
                    dataGridView2.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn11"].Value = product.Quantity;

                    double currentTotal = double.Parse(label5.Text.Split(' ')[0]);
                    double newTotal = currentTotal + product.Price;

                    label5.Text = newTotal.ToString("0.00") + " грн";
                }
            }

            if (e.ColumnIndex == dataGridView2.Columns["dataGridViewTextBoxColumn10"].Index && e.RowIndex >= 0)
            {
                if (Cart != null && Cart.ProductsInOrders != null && Cart.ProductsInOrders.Count > e.RowIndex)
                {
                    ProductsInOrders product = Cart.ProductsInOrders[e.RowIndex];
                    product.Quantity--;

                    if (product.Quantity == 0)
                    {
                        DialogResult result = MessageBox.Show("Ви дійсно бажаєте видалити з кошика товар '" + product.Name + "'?", "Підтвердження видалення", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            Cart.ProductsInOrders.RemoveAt(e.RowIndex);
                            RemoveRow(e.RowIndex);

                            double currentTotal = double.Parse(label5.Text.Split(' ')[0]);
                            double removedProductCost = product.Price;
                            double newTotal = currentTotal - removedProductCost;

                            label5.Text = newTotal.ToString("0.00") + " грн";
                        }
                        else
                        {
                            product.Quantity++;
                            dataGridView2.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn11"].Value = product.Quantity;
                        }
                    }
                    else
                    {
                        double currentTotal = double.Parse(label5.Text.Split(' ')[0]);
                        double newTotal = currentTotal - product.Price;
                        label5.Text = newTotal.ToString("0.00") + " грн";
                        dataGridView2.Rows[e.RowIndex].Cells["dataGridViewTextBoxColumn11"].Value = product.Quantity;
                    }
                }
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
                    e.Graphics.DrawRectangle(p, new System.Drawing.Rectangle(e.CellBounds.Location, new Size(e.CellBounds.Width - 1, e.CellBounds.Height - 1)));
                }

                if (e.Value != null)
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = (e.ColumnIndex == dataGridView2.Columns.Count - 1) ? StringAlignment.Center : StringAlignment.Near;
                    Brush textColor = (e.RowIndex == dataGridView2.CurrentCell.RowIndex && e.RowIndex >= 0 && e.ColumnIndex >= 0) ? Brushes.White : Brushes.Black;

                    e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font, textColor, e.CellBounds, sf);
                }

                e.Handled = true;
            }
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(51, 91, 49);
            button4.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 91, 49);
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {

            button4.BackColor = Color.FromArgb(76, 136, 74);
            button4.FlatAppearance.MouseOverBackColor = Color.FromArgb(76, 136, 74);
        }
    }
}
