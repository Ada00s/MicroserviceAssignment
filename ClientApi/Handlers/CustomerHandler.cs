using ClientApi.Handlers.Helpers;
using ClientApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ClientApi.Handlers
{
    public class CustomerHandler
    {
        private string CustomersRelativePath = @"Data\Customers";

        public async Task<bool> CreateNewCustomer(Customer newCustomer)
        {
            newCustomer.Id = FileHelper.GetLastId(CustomersRelativePath) + 1;
            try
            {
                var fileName = newCustomer.Id.ToString();
                var serializedCustomer = JsonConvert.SerializeObject(newCustomer);
                await FileHelper.AddOrOverwriteFile(CustomersRelativePath, fileName, serializedCustomer);
                return true; 
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            var storedCustomers = await FileHelper.GetAllDataForDirectory<Customer>(CustomersRelativePath);
            return storedCustomers.ToList();
        }

        public async Task<bool> UpdateCustomer(Customer existingCustomer)
        {
            var serializedOrder = JsonConvert.SerializeObject(existingCustomer);
            await FileHelper.AddOrOverwriteFile(CustomersRelativePath, $"{existingCustomer.Id}.json", serializedOrder);
            return true;
        }

        public async Task<Customer> GetCustomerById(int customerId)
        {
            var fileName = customerId.ToString() + ".json";
            return await FileHelper.GetObjectFromFile<Customer>(CustomersRelativePath, fileName);
        }


        
    }
}
