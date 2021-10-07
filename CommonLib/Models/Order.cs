using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CommonLib.Models
{
    public class Order
    {
        [JsonProperty(Required = Required.Always)]
        public int CustomerID { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int OrderId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public IDictionary<string, int> ProductsList { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTime OrderDate { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public ShipmentStatus Status { get; set; }
    } 

    public class OrderResponse
    {
        [JsonProperty(Required = Required.Always)]
        public int OrderId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public ShipmentStatus Status { get; set; }

        [JsonProperty(Required = Required.Always)]
        public double Price { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string Message { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
