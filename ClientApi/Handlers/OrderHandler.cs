using ClientApi.Handlers.Helpers;
using ClientApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;

namespace ClientApi.Handlers
{
    public interface IOrderHandler
    {
        Task<bool> CreateOrder(Order newOrder);
        Task<Order> GetOrderById(int orderId);
        Task<List<Order>> GetOrdersForCustomer(int customerId);
        Task<bool> UpdateOrderStatus(int orderId, int switchValue);
    }
    public class OrderHandler : IOrderHandler
    {
        private readonly string OrdersRelativePath = @"Data\Orders";
        public async Task<bool> CreateOrder (Order newOrder)
        {
            //TODO: Get customer to check if exists and check their billing situation (by status of last order)
            newOrder.OrderId = FileHelper.GetLastId(OrdersRelativePath) + 1;
            try
            {
                if(await SendOrder(newOrder))
                {
                    var fileName = newOrder.OrderId.ToString();
                    var serializedOrder = JsonConvert.SerializeObject(newOrder);
                    await FileHelper.AddOrOverwriteFile(OrdersRelativePath, fileName, serializedOrder);
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            var fileName = orderId.ToString() + ".json";
            return await FileHelper.GetObjectFromFile<Order>(OrdersRelativePath, fileName);
        }

        public async Task<List<Order>> GetOrdersForCustomer (int customerId)
        {
            bool exists = false; //TODO: Check if customer exists
            if (!exists)
            {
                throw new ApiException(HttpStatusCode.NotFound, $"Could not find customer with Id {customerId}");
            }
            var StoredOrders = await FileHelper.GetAllDataForDirectory<Order>(OrdersRelativePath);
            var results = (StoredOrders.Where(order => order.CustomerID == customerId)).ToList();
            return results;
        }

        public async Task<bool> UpdateOrderStatus(int orderId, int switchValue)
        {
            var order = await GetOrderById(orderId);
            if (order == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, $"Order with id {orderId} could not be found");
            }
            order.Status = (ShipmentStatus)switchValue;
            var serializedOrder = JsonConvert.SerializeObject(order);
            await FileHelper.AddOrOverwriteFile(OrdersRelativePath, $"{orderId.ToString()}.json", serializedOrder);
            return true;
        }

        private async Task<bool> SendOrder(Order newOrder)
        {
            //TODO: Messaging to the warehouse. Returns true if warehouse can process this order
            return false;
        }
    }
}
