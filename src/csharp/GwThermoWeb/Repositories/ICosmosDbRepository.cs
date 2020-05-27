using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation.Models;

namespace GwThermoWeb.Repositories
{
    public interface ICosmosDbRepository
    {
        Task Initialize();
        Task<IList<ThermoTelemetryData>> GetDataAsync(string deviceId, int top = 100);
    }
}
