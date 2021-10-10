using CommonLib.Models;
using EasyNetQ;
using MicroserviceAssignment.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroserviceAssignment
{
    public static class Subscriber 
    {
        public static async Task Run(CancellationToken token)
        {
            Console.WriteLine("Starting the Subscriber");
            while (!token.IsCancellationRequested)
            {
                var ordersHandler = new OrdersHandler();
                var bus = RabbitHutch.CreateBus($"host = { Settings.Host}; virtualHost = {Settings.VirualHost}; username = {Settings.VirualHost}; password = {Settings.Password}");
                await bus.Rpc.RespondAsync<Order, OrderResponse>(async (r) => await ordersHandler.HandleOrder(r));
            }
        }

        private static class Settings
        {
            public const string Host = "cow.rmq2.cloudamqp.com";
            public const string VirualHost = "sgjupbsr";
            public const string Password = "YPG6BrZzn9WOWlzhopFZIr4bXKzz62zg";
        }
    }
}
