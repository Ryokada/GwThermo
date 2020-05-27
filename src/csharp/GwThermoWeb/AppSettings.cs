using System;
namespace GwThermoWeb
{
    public class AppSettings
    {
        public CosmosDB CosmosDB { get; set; }
    }
    public class CosmosDB
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
    }
}
