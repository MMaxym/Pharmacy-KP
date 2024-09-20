using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmacy
{
    public interface IProductsInOrders
    {
        string Name { get; set; }
        double Price { get; set; }
        int Quantity { get; set; }
    }

    public class ProductsInOrders: IProductsInOrders
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public ProductsInOrders() { }

        public ProductsInOrders(string name, double price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public Double getPrice()
        {
            return this.Price * this.Quantity;
        }

        public String GetAllInformation()
        {
            return $"{Name} {Price} {Quantity}";
        }

        public void ReduceStock(int quantityToReduce)
        {
            if (quantityToReduce > Quantity)
            {
                MessageBox.Show("Кількість для зменшення перевищує поточний залишок !!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Quantity -= quantityToReduce;
        }

        public void IncreaseStock(int quantityToAdd)
        {
            Quantity += quantityToAdd;
        }

        public void UpdatePriceByName(string productName, double newPrice)
        {
            if (Name == productName)
            {
                Price = newPrice;
            }
        }

        public bool IsProductInStock(string productName)
        {
            return Name == productName && Quantity > 0;
        }

        public bool IsSameProduct(ProductsInOrders otherProduct, bool includeQuantity = false)
        {
            bool nameMatches = Name == otherProduct.Name;
            bool priceMatches = Price == otherProduct.Price;

            if (includeQuantity)
            {
                bool quantityMatches = Quantity == otherProduct.Quantity;
                return nameMatches && priceMatches && quantityMatches;
            }

            return nameMatches && priceMatches;
        }
    } 
}
