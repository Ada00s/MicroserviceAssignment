using System;
using System.Collections.Generic;

namespace ClientApi.Models
{
    public abstract class Order
    {
        public int CustomerID { get; set; }
        public IEnumerable<Product> ProductsList { get; set; }
    }
    public class CreateOrder : Order
    {

    }
    public class ReadOrder : Order
    {
        public ShipmentStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
