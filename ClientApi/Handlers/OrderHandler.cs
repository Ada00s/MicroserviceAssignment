using CommonLib.Helpers;
using CommonLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using EasyNetQ;
using Microsoft.Extensions.Options;

namespace ClientApi.Handlers
{
    public interface IOrderHandler
    {
        Task<OrderResponse> CreateOrder(NewOrder newOrder);
        Task<Order> GetOrderById(int orderId);
        Task<List<Order>> GetOrdersForCustomer(int customerId);
        Task<bool> UpdateOrderStatus(int orderId, int switchValue);
    }
    public class OrderHandler : IOrderHandler
    {
        private readonly string OrdersRelativePath = @"Data\Orders";
        private readonly ICustomerHandler _customerHandler;
        private readonly IOptions<ApiConfig> _config;
        private readonly IBus _bus;

        public OrderHandler(ICustomerHandler handler, IOptions<ApiConfig> config)
        {
            _customerHandler = handler;
            _config = config;
            _bus = RabbitHutch.CreateBus($"host={ _config.Value.Host};virtualHost={_config.Value.VUser};username={_config.Value.VUser};password={_config.Value.Password}");
        }

        public async Task<OrderResponse> CreateOrder (NewOrder newOrder)
        {
            var CustomerHistory = await CheckCustomerCreditability(newOrder.CustomerID);
            if(CustomerHistory.Count > 0)
            {
                throw new ApiException(HttpStatusCode.Forbidden, $"Customer has {CustomerHistory.Count} unpaid order(s).");
            }
            var order = new Order { CustomerID = newOrder.CustomerID, ProductsList = newOrder.ProductsList };
            order.OrderId = FileHelper.GetLastId(OrdersRelativePath) + 1;
            order.OrderDate = DateTime.Now;
            try
            {
                var response = await SendOrder(order);
                if (response.Status == ShipmentStatus.Cancelled)
                {
                    throw new ApiException(HttpStatusCode.NotAcceptable, $"Warehouse could not process the shipment. Message: {response.Message}");
                }
                order.Status = response.Status;
                var fileName = order.OrderId.ToString();
                var serializedOrder = JsonConvert.SerializeObject(order);
                await FileHelper.AddOrOverwriteFile(OrdersRelativePath, fileName, serializedOrder);
                return response;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            var fileName = orderId.ToString() + ".json";
            return await FileHelper.GetObjectFromFile<Order>(OrdersRelativePath, fileName);
        }

        public async Task<List<Order>> GetOrdersForCustomer (int customerId)
        {
            bool exists = !(await _customerHandler.GetCustomerById(customerId)==null);
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
            try
            {
                await SignalStatusUpdate(new StatusSignal { OrderId = orderId, Status = order.Status });

                await FileHelper.AddOrOverwriteFile(OrdersRelativePath, $"{orderId.ToString()}.json", serializedOrder);
                return true;
            }catch (Exception e)
            {
                return false;
            }
        }

        private async Task<OrderResponse> SendOrder(Order newOrder)
        {
            return await _bus.Rpc.RequestAsync<Order, OrderResponse>(newOrder);
        }
        private async Task<StatusSignal> SignalStatusUpdate(StatusSignal status)
        {
            var order = await GetOrderById(status.OrderId);
            var statusMessage = new StatusSignalMessage { OrderId = status.OrderId, ProductsList = order.ProductsList, Status = status.Status };
            return await _bus.Rpc.RequestAsync<StatusSignalMessage, StatusSignal>(statusMessage);
        }

        /// <summary>
        /// Return null if customer has everything paid
        /// </summary>
        public async Task<List<Order>> CheckCustomerCreditability(int customerId)
        {
            try
            {
                var customerOrders = await GetOrdersForCustomer(customerId);

                var UnpaidOrders = new List<Order>();
                foreach (var ord in customerOrders)
                {
                    if (ord.Status != ShipmentStatus.Paid || ord.Status != ShipmentStatus.Cancelled)
                    {
                        UnpaidOrders.Add(ord);
                    }
                }
                return UnpaidOrders;
            }
            catch (ApiException)
            {
                return null;
            }
        }
    }
}
