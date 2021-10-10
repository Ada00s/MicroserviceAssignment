using System;
using System.Threading.Tasks;

namespace MicroserviceAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
            Subscriber.Run(new System.Threading.CancellationToken(false));
        }
    }
}
