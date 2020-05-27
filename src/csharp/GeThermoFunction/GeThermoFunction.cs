using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Foundation.Models;
using System.Threading.Tasks;

namespace GwThermoFunction
{
    public static class GwThermoFunction
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("GeThermoFunction")]
        public static async Task Run(
            [IoTHubTrigger("messages/events", Connection = "IotHubConnectionSettings", ConsumerGroup = "function")]EventData message,
            [CosmosDB(
                databaseName: "gwthermodb",
                collectionName: "gwthermocontainer",
                ConnectionStringSetting = "CosmosDBConnectionSettings")]
            IAsyncCollector<ThermoTelemetryData> dataOut,
            ILogger logger
        )
        {
            var rawTelemetry = Encoding.UTF8.GetString(message.Body.Array);
            logger.LogDebug($"C# IoT Hub trigger function processed a message: {rawTelemetry}");

            var telemetry = JsonConvert.DeserializeObject<ThermoTelemetryData>(rawTelemetry);
            telemetry.DeviceId = message.SystemProperties["iothub-connection-device-id"]?.ToString() ?? "TestDevice";
             
            if (message.Properties.TryGetValue("anomaly", out var anomaly))
            {
                if (anomaly.ToString() == "true")
                {
                    // プッシュ通知
                    logger.LogWarning("Anomaly!!!!!!!");
                }
                logger.LogDebug($"Anomaly: {anomaly}");
            }

            await dataOut.AddAsync(telemetry).ConfigureAwait(false);
            logger.LogInformation($"Save to Cosmos DB");
        }
    }
}