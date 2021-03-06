using CommonLib.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroserviceAssignment.Handlers
{
    public class OrdersHandler
    {
        public async Task<OrderResponse> HandleOrder(Order newOrder)
        {
            var response = new OrderResponse();
            Console.WriteLine($"Processing Order with ID {newOrder.OrderId}");
            try
            {
                if (!await CheckIfInStock(newOrder))
                {
                    return new OrderResponse { Message = "Not enough items in stock", OrderId = newOrder.OrderId, Price = 0, Status = ShipmentStatus.Cancelled };
                }
                else
                {
                    var price = await CalculatePrice(newOrder);
                    ReserveProducts(newOrder);
                    return new OrderResponse { Message = $"Shipped order with an Id {newOrder.OrderId}. Price: {price}.", OrderId = newOrder.OrderId, Price = price, Status = ShipmentStatus.Shipped };
                }
            }
            catch (Exception e)
            {
                return new OrderResponse { Message = $"Encountered error: {e.Message}", OrderId = newOrder.OrderId, Price = 0, Status = ShipmentStatus.Cancelled };
            }
        }

        public async Task<StatusSignal> HandleStatusSignal(StatusSignalMessage status)
        {
            Console.WriteLine($"Processing StatusChange for Order with ID {status.OrderId}");
            switch (status.Status)
            {
                case ShipmentStatus.Cancelled:
                    {
                        await WarehouseHandler.CancellOrder(status.ProductsList);
                        break;
                    }
                case ShipmentStatus.Paid :
                {
                        await WarehouseHandler.CompleteOrder(status.ProductsList);
                        break;
                }
            }
            return new StatusSignal { OrderId = status.OrderId, Status = status.Status };
            
        }

        /// <summary>
        /// Returns false if anything is missing from a stock
        /// </summary>
        private async Task<bool> CheckIfInStock(Order order)
        {
            foreach (var prod in order.ProductsList)
            {
                if((await WarehouseHandler.GetNumberInStock(prod.Key) - prod.Value) <=0)
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
                result += (await WarehouseHandler.GetItemPrice(prod.Key) * prod.Value);
            }
            return result;
        }

        private void ReserveProducts(Order order)
        {
            List<Task> reservations = new List<Task>();
            foreach(var prod in order.ProductsList)
            {
                reservations.Add(WarehouseHandler.ReserveProduct(prod.Key, prod.Value));
            }
            Task.WaitAll(reservations.ToArray());
        }
    }
}
