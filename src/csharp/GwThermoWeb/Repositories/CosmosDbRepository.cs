using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GwThermoWeb.Repositories
{
    public class CosmosDbRepository : ICosmosDbRepository
    {
        private static readonly string DetabaseId = "gwthermodb";
        private static readonly string ContainerId = "gwthermocontainer";


        private readonly ILogger<CosmosDbRepository> _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly AppSettings _appSettings;

        // The database we will create
        private Database _database;

        // The container we will create.
        private Container _container;

        public CosmosDbRepository(IOptions<AppSettings> optionsAccessor, ILogger<CosmosDbRepository> logger)
        {
            this._logger = logger;
            this._appSettings = optionsAccessor.Value;
            this._cosmosClient = new CosmosClient(this._appSettings.CosmosDB.EndpointUri, this._appSettings.CosmosDB.PrimaryKey);
            Initialize().GetAwaiter().GetResult();
        }

        public async Task Initialize()
        {
            this._database = await this._cosmosClient.CreateDatabaseIfNotExistsAsync(DetabaseId).ConfigureAwait(false);
            this._logger.LogInformation("Created Database: {0}\n", this._database.Id);

            this._container = await this._database.CreateContainerIfNotExistsAsync(ContainerId, "/messagetype").ConfigureAwait(false);
            this._logger.LogInformation("Created Container: {0}\n", this._container.Id);
        }

        public async Task<IList<ThermoTelemetryData>> GetDataAsync(string deviceId, int top = 100)
        {
            var query =
                new QueryDefinition("select * from t where t.deviceId = @deviceId ORDER BY t.timestamp DESC OFFSET 0 LIMIT @top")
                    .WithParameter("@deviceId", deviceId).WithParameter("@top", top);

            var telemetriesIterator = this._container.GetItemQueryIterator<ThermoTelemetryData>(query);

            var telemetries = new List<ThermoTelemetryData>();
            while (telemetriesIterator.HasMoreResults)
            {
                var resultSet = await telemetriesIterator.ReadNextAsync().ConfigureAwait(false);
                foreach (var telemetry in resultSet)
                {
                    telemetries.Add(telemetry);
                }
            }

            return telemetries;
        }
    }
}
