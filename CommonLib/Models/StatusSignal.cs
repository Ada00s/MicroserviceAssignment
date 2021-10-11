using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.Models
{
    public class StatusSignal
    {
        [JsonProperty(Required = Required.Always)]
        public int OrderId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public ShipmentStatus Status { get; set; }
    }
    public class StatusSignalMessage
    {
        [JsonProperty(Required = Required.Always)]
        public int OrderId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public ShipmentStatus Status { get; set; }

        [JsonProperty(Required = Required.Always)]
        public IDictionary<string, int> ProductsList { get; set; }
    }

}
