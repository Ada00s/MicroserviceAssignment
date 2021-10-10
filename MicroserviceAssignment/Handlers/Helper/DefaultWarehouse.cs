using CommonLib.Helpers;
using CommonLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceAssignment.Handlers.Helper
{
    /// <summary>
    /// Little hack around the fact that we are not working on the DB. Contains object of default data for warehouse
    /// </summary>
    public static class DefaultWarehouse
    {
        private static string RelativePath = Path.Combine("Data", "Stock");
        private static int _lastId;
        private static IList<Product> Data;

        public static async Task<bool> Initialize()
        {
            _lastId = 1000;
            Data = new List<Product>();
            AddProduct("Product#1", 25.99, 100);
            AddProduct("Product#2", 55.19, 60);
            AddProduct("Product#3", 99.99, 40);
            AddProduct("Product#4", 19.50, 300);
            AddProduct("Product#5", 40.99, 70);
            AddProduct("Product#6", 60, 100);
            AddProduct("Product#7", 75.15, 45);
            AddProduct("Product#8", 35.19, 110);
            AddProduct("Product#9", 115.99, 30);
            AddProduct("Product#10", 29.99, 120);
            AddProduct("Luxury#1", 599.50, 10);
            AddProduct("Luxury#2", 399.50, 20);
            AddProduct("Luxury#3", 5599.50, 5);
            AddProduct("Luxury#4", 3599.50, 5);

            foreach (var prod in Data)
            {
                await FileHelper.AddOrOverwriteFile(RelativePath, prod.Name, JsonConvert.SerializeObject(prod));
            }
            return true;
        }

        private static void AddProduct(string NName, double NPrice, int NNumberInStock)
        {
            Data.Add(new Product
            {
                Id = _lastId,
                Name = NName,
                NumberInStock = NNumberInStock,
                Price = NPrice,
                Reserved = 0 // in initialization no products are being reserved
            });
            _lastId++;
        }
    }
}
