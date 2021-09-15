using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ClientApi.Models
{
    public class Order
    {
        [JsonProperty(Required = Required.Always)]
        public int CustomerID { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int OrderId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public IDictionary<Product, int> ProductsList { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTime OrderDate { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public ShipmentStatus Status { get; set; }
    } 
}
