using CommonLib.Helpers;
using CommonLib.Models;
using MicroserviceAssignment.Handlers.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceAssignment.Handlers
{
    public class WarehouseHandler
    {
        private static string RelativePath = @"Data\Stock";

        /// <summary>
        /// Check if any data exists, if not, generates default. Return true when data is present or successfully initialized.
        /// </summary>
        public async Task<bool> DataInitialization()
        {
            if (FileHelper.CheckIfDataExists(RelativePath))
            {
                return true;//Files already exists
            }
            try
            {
                await DefaultWarehouse.Initialize();
                return true;

            }catch (Exception)
            {
                return false;
            }
        }

        public async Task<Product> GetProduct(string ProductName)
        {
            return await FileHelper.GetObjectFromFile<Product>(RelativePath, ProductName);
        }

        public async Task ReserveProduct(string product, int Reserve)
        {
            var prod = await GetProduct(product);
            prod.Reserved = +Reserve;
            await FileHelper.AddOrOverwriteFile(RelativePath, product, JsonConvert.SerializeObject(prod));
        }

        public async Task CompleteOrder(string product, int Reserved)
        {
            var prod = await GetProduct(product);
            prod.NumberInStock -= Reserved;
            prod.Reserved -= Reserved;
            await FileHelper.AddOrOverwriteFile(RelativePath, product, JsonConvert.SerializeObject(prod));
        }

        public async Task<int> GetNumberInStock(string product)
        {
            return (await GetProduct(product)).NumberInStock;
        }

        public async Task<double> GetItemPrice(string product)
        {
            return (await GetProduct(product)).Price;
        }
    }
}
