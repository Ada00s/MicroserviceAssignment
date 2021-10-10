using CommonLib.Helpers;
using CommonLib.Models;
using MicroserviceAssignment.Handlers.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceAssignment.Handlers
{
    public static class WarehouseHandler
    {
        private static string RelativePath = Path.Combine("Data", "Stock");

        /// <summary>
        /// Check if any data exists, if not, generates default. Return true when data is present or successfully initialized.
        /// </summary>
        public static async Task DataInitialization()
        {
            if (FileHelper.CheckIfDataExists(RelativePath))
            {
                Console.WriteLine("Data already initialized");
                return;
            }
            try
            {
                await DefaultWarehouse.Initialize();
                Console.WriteLine("Data has been initialized");
                return;

            }catch (Exception)
            {
                Console.WriteLine("Data could not be initialized");
                return;
            }
        }

        public static async Task<Product> GetProduct(string ProductName)
        {
            try 
            { 
                return await FileHelper.GetObjectFromFile<Product>(RelativePath, ProductName); 
            }
            catch (ApiException)
            {
                return null;
            }
        }

        public static async Task ReserveProduct(string product, int Reserve)
        {
            var prod = await GetProduct(product);
            prod.Reserved = +Reserve;
            await FileHelper.AddOrOverwriteFile(RelativePath, product, JsonConvert.SerializeObject(prod));
        }

        public static async Task CancellOrder(IDictionary<string, int> products)
        {
            foreach (var prod in products)
            {
                var prodTemp = await GetProduct(prod.Key);
                prodTemp.Reserved = -prod.Value;
                await FileHelper.AddOrOverwriteFile(RelativePath, prod.Key, JsonConvert.SerializeObject(prodTemp));
            }
        }

        public static async Task CompleteOrder(IDictionary<string, int> products)
        {
           foreach (var prod in products)
            {
                var prodTemp = await GetProduct(prod.Key);
                prodTemp.NumberInStock -= prod.Value;
                prodTemp.Reserved -= prod.Value;
                await FileHelper.AddOrOverwriteFile(RelativePath, prod.Key, JsonConvert.SerializeObject(prodTemp));
            }
        }

        public static async Task<int> GetNumberInStock(string product)
        {
            var productInfo = await GetProduct(product);
            if(productInfo == null)
            {
                return 0;
            }
            return productInfo.NumberInStock - productInfo.Reserved;
        }

        public static async Task<double> GetItemPrice(string product)
        {
            return (await GetProduct(product)).Price;
        }
    }
}
