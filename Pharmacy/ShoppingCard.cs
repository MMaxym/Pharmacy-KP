using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmacy
{
    public interface IShoppingCart
    {
        string Name { get; set; }
        string PhoneNumber { get; set; }
        List<ProductsInOrders> ProductsInOrders { get; set; }
    }

    public class ShoppingCart: IShoppingCart
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public List<ProductsInOrders> ProductsInOrders { get; set; }

        public ShoppingCart() { }

        public ShoppingCart(string name, string phoneNumber, List<ProductsInOrders> productsInOrders)
        {
           Name = name;
           PhoneNumber = phoneNumber;
           ProductsInOrders = productsInOrders;
        }

        public void AddProductToCart(ProductsInOrders product)
        {
            ProductsInOrders.Add(product);
        }

        public void RemoveProductFromCart(int index)
        {
            if (index >= 0 && index < ProductsInOrders.Count)
            {
                ProductsInOrders.RemoveAt(index);
            }
            else
            {
                MessageBox.Show("Такого товару немає в Вашому кошику!!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public double CalculateTotalPrice()
        {
            double totalPrice = 0;
            foreach (var product in ProductsInOrders)
            {
                totalPrice += product.Price * product.Quantity;
            }
            return totalPrice;
        }

        public int CountTotalItemsInCart()
        {
            int totalItems = 0;
            foreach (var product in ProductsInOrders)
            {
                totalItems += product.Quantity;
            }
            return totalItems;
        }

        public void UpdateProductQuantity(int index, int newQuantity)
        {
            if (index >= 0 && index < ProductsInOrders.Count)
            {
                if (newQuantity >= 0)
                {
                    ProductsInOrders[index].Quantity = newQuantity;
                }
                else
                {
                    MessageBox.Show("Кількість не може бути від'ємною!!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Такого товару немає в Вашому кошику!!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public void SaveCartToFile(string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
                File.WriteAllText(filePath, json);

                MessageBox.Show("Інформацію про кошик збережено успішно!!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження інформації про кошик: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadCartFromFile(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                ShoppingCart cart = JsonConvert.DeserializeObject<ShoppingCart>(json);

                this.Name = cart.Name;
                this.PhoneNumber = cart.PhoneNumber;
                this.ProductsInOrders = cart.ProductsInOrders;

                MessageBox.Show("Інформацію про кошик завантажено успішно!!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження інформації про кошик: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }   
}
