using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmacy
{
    public partial class Form5 : Form
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public List<ProductsInOrders> products = new List<ProductsInOrders>();
        public ShoppingCart Cart { get; set; }

        public Form5()
        {
            InitializeComponent();

            this.MaximizeBox = false;

            numericUpDown1.Maximum = 100000;
            numericUpDown1.TabStop = false;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            label2.Text = Name;
            label4.Text = Price.ToString();
            label7.Text = Quantity.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int quantity1 = (int)numericUpDown1.Value;

            if (quantity1 == 0)
            {
                MessageBox.Show("Не введена кількість товару!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (quantity1 > Quantity)
            {
                MessageBox.Show("Недостатня кількість товару на складі!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (ProductsInOrders product in Cart.ProductsInOrders)
            {
                if (product.Name == Name)
                {
                    product.Quantity += quantity1;
                    MessageBox.Show($"Товар '{Name}' вже є у Вашому кошику!\n\nКількість товару '{Name}' у Вашому кошику була збільшена на {quantity1}!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }
            }

            ProductsInOrders newProduct = new ProductsInOrders(Name, Price, quantity1);
            Cart.ProductsInOrders.Add(newProduct);

            MessageBox.Show($"Товар '{Name}' додано у Ваш кошик!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(76, 136, 74);
            button4.FlatAppearance.MouseOverBackColor = Color.FromArgb(76, 136, 74);
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(51, 91, 49);
            button4.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 91, 49);
        }
    }
}