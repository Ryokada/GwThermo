using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation.Models;
using GwThermoWeb.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GwThermoWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThermoController : Controller
    {
        private readonly ILogger<ThermoController> _logger;
        private readonly ICosmosDbRepository _repository;
        private readonly AppSettings _appSettings;

        public ThermoController(IOptions<AppSettings> optionsAccessor, ICosmosDbRepository repository, ILogger<ThermoController> logger)
        {
            this._logger = logger;
            this._repository = repository;
            this._appSettings = optionsAccessor.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("getdata/{deviceid}")]
        public async Task<IActionResult> GetTermoData(string deviceId, [FromQuery] int top = 100)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                return BadRequest();
            }

            var results = await this._repository.GetDataAsync(deviceId, top).ConfigureAwait(false);
            Response.Headers["Access-Control-Allow-Origin"] = "*";

            return Ok(results);
        }

        [Route("getdata/latest/{deviceid}")]
        public async Task<IActionResult> GetLatestTermoData(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                return BadRequest();
            }

            var results = await this._repository.GetDataAsync(deviceId, 1).ConfigureAwait(false);
            Response.Headers["Access-Control-Allow-Origin"] = "*";

            return Ok(results.First());
        }
    }
}