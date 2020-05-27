using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Foundation.Models
{
    public class ThermoTelemetryData
    {
        [JsonProperty("deviceId")]
        public String DeviceId { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("data")]
        public IReadOnlyList<IReadOnlyList<float>> Data { get; set; }

        [JsonProperty("maxValue", NullValueHandling = NullValueHandling.Ignore)]
        public float? MaxValue { get; set; }
    }
}
