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
    public interface IOrders
    {
        int Number { get; set; }
        string Name { get; set; }
        string PhoneNumber { get; set; }
        string DateTime { get; set; }
        string Status { get; set; }
        string Payment {  get; set; }
        string Suma { get; set; }
        List<ProductsInOrders> Products { get; set; }
    }

    public class Orders : IOrders
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string DateTime {  get; set; }
        public string Status { get; set; }
        public string Payment { get; set; }
        public string Suma { get; set; }
        public List<ProductsInOrders> Products { get; set; }

        public Orders(int number, string name, string phoneNumber, string dateTime, string status, string payment, string suma, List<ProductsInOrders> products)
        {
            Number = number;
            Name = name;
            PhoneNumber = phoneNumber;
            DateTime = dateTime;
            Status = status;
            Payment = payment;
            Suma = suma;
            Products = products;
        }

        public static void SaveOrderToJson(Orders order, string jsonFileName)
        {
            List<Orders> ordersList = new List<Orders>();

            if (File.Exists(jsonFileName))
            {
                string jsonData = File.ReadAllText(jsonFileName);

                ordersList = JsonConvert.DeserializeObject<List<Orders>>(jsonData);
            }

            ordersList.Add(order);

            string updatedJson = JsonConvert.SerializeObject(ordersList, Formatting.Indented);
            File.WriteAllText(jsonFileName, updatedJson);
        }

        public static List<Orders> LoadOrdersFromJson(string jsonFileName)
        {
            List<Orders> ordersList = new List<Orders>();

            try
            {
                if (File.Exists(jsonFileName))
                {
                    string jsonData = File.ReadAllText(jsonFileName);
                    ordersList = JsonConvert.DeserializeObject<List<Orders>>(jsonData);
                }
                else
                {
                    MessageBox.Show("Файл замовлень не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при читанні з файлу замовлень: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ordersList;
        }

        public double CalculateOrderTotal()
        {
            double total = 0;
            foreach (var product in Products)
            {
                total += product.Price * product.Quantity;
            }
            return total;
        }

        public bool ContainsProduct(string productName)
        {
            foreach (var product in Products)
            {
                if (product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public List<ProductsInOrders> GetProductsBelowQuantity(int quantity)
        {
            List<ProductsInOrders> productsBelowQuantity = new List<ProductsInOrders>();
            foreach (var product in Products)
            {
                if (product.Quantity < quantity)
                {
                    productsBelowQuantity.Add(product);
                }
            }
            return productsBelowQuantity;
        }


        public void UpdateOrderStatus(string newStatus)
        {
            Status = newStatus;
            MessageBox.Show($"Статус замовлення оновлено: {newStatus}", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ClearOrder()
        {
            Products.Clear();
            MessageBox.Show("Замовлення очищено.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ProcessPayment()
        {
            Payment = "Оплачено";
            MessageBox.Show("Оплата замовлення успішно проведена.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

