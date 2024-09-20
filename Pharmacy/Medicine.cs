using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmacy
{
    public interface IMedicine
    {
        string Name { get; set; }
        string Category { get; set; }
        double Price { get; set; }
        string ExpiryDate { get; set; }
        int QuantityAvailable { get; set; }
        bool Prescription { get; set; }
        string Instruction { get; set; }
        string Provider { get; set; }
    }

    public class Medicine : IMedicine
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public string ExpiryDate { get; set; }
        public int QuantityAvailable { get; set; }
        public bool Prescription { get; set; }
        public string Instruction { get; set; }
        public string Provider { get; set; }

        public Medicine() { }

        public Medicine(string name, string category, double price, string expiryDate, int quantityAvailable, bool prescription, string instruction, string provider)
        {
            Name = name;
            Category = category;
            Price = price;
            ExpiryDate = expiryDate;
            QuantityAvailable = quantityAvailable;
            Prescription = prescription;
            Instruction = instruction;
            Provider = provider;
        }

        public String GetNameAndCategory()
        {
            return $"{Name} ({Category})";
        }

        public bool RequiresPrescription()
        {
            return Prescription;
        }

        public void ReduceQuantity(int quantityToReduce)
        {
            if (quantityToReduce > QuantityAvailable)
            {
                MessageBox.Show("Кількість для зменшення перевищує поточний залишок !!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            QuantityAvailable -= quantityToReduce;
        }

        public void UpdateInstruction(string newInstruction)
        {
            Instruction = newInstruction;
        }

        public bool IsExpired()
        {
            DateTime expiryDateTime = DateTime.ParseExact(ExpiryDate, "dd/MM/yyyy", null);
            return expiryDateTime <= DateTime.Now;
        }

        public bool IsPriceInRange(double minPrice, double maxPrice)
        {
            return Price >= minPrice && Price <= maxPrice;
        }

        public void IncreaseQuantity(int quantityToAdd)
        {
            QuantityAvailable += quantityToAdd;
        }

        public void Reserve(int quantityToReserve)
        {
            if (quantityToReserve > QuantityAvailable)
            {
                MessageBox.Show("Кількість для резервування перевищує доступну кількість !!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            QuantityAvailable -= quantityToReserve;
            Console.WriteLine($"Зарезервовано {quantityToReserve} одиниць товару '{Name}'");
        }

        public bool CanBeUsedWith(Medicine otherMedicine)
        {
            if (Provider != otherMedicine.Provider)
            {
                return false;
            }

            if (Prescription || otherMedicine.Prescription)
            {
                return false;
            }
            return Category != otherMedicine.Category && ExpiryDate == otherMedicine.ExpiryDate;
        }
    }
}
