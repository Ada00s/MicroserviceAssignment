using CommonLib.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceAssignment.Handlers
{
    public class OrdersHandler
    {
        private WarehouseHandler _warehouseHandler { get; set; }

        public OrdersHandler(WarehouseHandler handler)
        {
            _warehouseHandler = handler;
        }

        public async Task<OrderResponse> HandleOrder(Order newOrder)
        {
            try
            {
                if (!await CheckIfInStock(newOrder))
                {
                    return new OrderResponse { Message = "Not enough items in stock", OrderId = newOrder.OrderId, Price = 0, Status = ShipmentStatus.Cancelled };
                }

                var price = await CalculatePrice(newOrder);
                ReserveProducts(newOrder);
                return new OrderResponse { Message = $"Shipped order with an Id {newOrder.OrderId}. Price: {price}.", OrderId = newOrder.OrderId, Price = price, Status = ShipmentStatus.Shipped };
            }catch (Exception e)
            {
                return new OrderResponse { Message = $"Encountered error: {e.Message}", OrderId = newOrder.OrderId, Price = 0, Status = ShipmentStatus.Cancelled };
            }
           
        }

        /// <summary>
        /// Returns false if anything is missing from a stock
        /// </summary>
        private async Task<bool> CheckIfInStock(Order order)
        {
            foreach (var prod in order.ProductsList)
            {
                if((await _warehouseHandler.GetNumberInStock(prod.Key) - prod.Value) <0)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<double> CalculatePrice(Order order)
        {
            var result = 0.0;
            foreach (var prod in order.ProductsList)
            {
                result += (await _warehouseHandler.GetItemPrice(prod.Key) * prod.Value);
            }
            return result;
        }

        private void ReserveProducts(Order order)
        {
            List<Task> reservations = new List<Task>();
            foreach(var prod in order.ProductsList)
            {
                reservations.Add(_warehouseHandler.ReserveProduct(prod.Key, prod.Value));
            }
            Task.WaitAll(reservations.ToArray());
        }
    }
}
