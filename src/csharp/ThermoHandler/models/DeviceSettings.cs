using System;
using Newtonsoft.Json;

namespace ThermoHandler.models
{
    public class DeviceSettings
    {
        [JsonProperty("connectionString")]
        public String ConnectionString { get; set; }

        [JsonProperty("ledPinNumber")]
        public int LedPinNumber { get; set; }

        [JsonProperty("i2cBusId")]
        public int I2cBusId { get; set; }

        [JsonProperty("i2xDeviceAddress")]
        public int I2cDeviceAddress { get; set; }

        [JsonProperty("telemetryInterval")]
        public TimeSpan TelemetryInterval { get; set; }

        [JsonProperty("collectionInterval")]
        public TimeSpan CollectionInterval { get; set; }

        [JsonProperty("threshold")]
        public float Threshold { get; set; }
    }
}
