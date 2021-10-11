using MicroserviceAssignment.Handlers;
using System;
using System.Threading.Tasks;

namespace MicroserviceAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.WaitAll(WarehouseHandler.DataInitialization());
            var subscriber = Task.Run(() => Subscriber.Run());
            subscriber.Wait();
        }
    }
}
