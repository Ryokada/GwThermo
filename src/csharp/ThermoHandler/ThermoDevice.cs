using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using ThermoHandler.models;
using System.Diagnostics;
using System.Collections.Generic;
using Foundation.Models;
using System.Linq;
using Foundation;
using Microsoft.Azure.Devices.Client.Exceptions;

namespace ThermoHandler
{
    public class ThermoDevice : IDisposable
    {
        private readonly DeviceClient _client;
        private readonly ILogger _logger;
        private readonly ThermoSensor _thermoSensor;
        private readonly Led _led;
        private readonly float _threshold;
        private readonly TimeSpan _collectInterval = TimeSpan.MaxValue;
        private readonly TimeSpan _telemetryInterval = TimeSpan.MaxValue;

        private readonly SemaphoreSlim _chachLock = new SemaphoreSlim(0, 1);
        private ThermoTelemetryData _chacheData;

        public ThermoDevice(DeviceSettings settings, ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<ThermoDevice>();
            this._threshold = settings.Threshold;
            this._collectInterval = settings.CollectionInterval;
            this._telemetryInterval = settings.TelemetryInterval;

            // 各種セットアップ
            this._client = DeviceClient.CreateFromConnectionString(settings.ConnectionString, TransportType.Mqtt);

            this._led = new Led(loggerFactory);
            this._led.Initialize(settings.LedPinNumber);

            this._thermoSensor = new ThermoSensor(loggerFactory);
            this._thermoSensor.Initialize(settings.I2cBusId, settings.I2cDeviceAddress);

        }

        public void Dispose()
        {
            this._thermoSensor.Dispose();
            this._led.Dispose();
            this._client.Dispose();
        }

        public async Task StertAsync(CancellationToken token)
        {
            this._chachLock.Release();
            await this._client.SetMethodHandlerAsync("GetThermo", this.GetNowThermoDataAsync, null).ConfigureAwait(false);

#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます
            CollectDataAsync(token);
#pragma warning restore CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

            await sendTelemetryAsync(token).ConfigureAwait(false);
        }

        private async Task CollectDataAsync(CancellationToken token)
        {
            var stopwatch = Stopwatch.StartNew();
            while (!token.IsCancellationRequested)
            {
                stopwatch.Restart();

                // データ取得
                var thermoValue = this._thermoSensor.ReadThermo();
                var telemetory = new ThermoTelemetryData()
                {
                    Timestamp = DateTimeOffset.UtcNow,
                    Data = thermoValue,
                    MaxValue = this.GetMaxValue(thermoValue)
                };

                // キャッシュ
                await this._chachLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (this._chacheData == null)
                    {
                        this._chacheData = telemetory;
                        this._logger.LogInformation($"{DateTime.UtcNow} > Collect data: {JsonConvert.SerializeObject(telemetory)}");
                    }
                    else
                    {
                        if (this._chacheData.MaxValue <= telemetory.MaxValue)
                        {
                            this._chacheData = telemetory;
                            this._logger.LogInformation($"{DateTime.UtcNow} > Collect data: {JsonConvert.SerializeObject(telemetory)}");
                        }
                    }
                }
                finally
                {
                    this._chachLock.Release();
                }


                // まつ。
                var interval = this._collectInterval.Ticks - stopwatch.Elapsed.Ticks;
                await Task.Delay(interval > 0 ? TimeSpan.FromTicks(interval) : TimeSpan.Zero, token);
            }
        }

        private async Task sendTelemetryAsync(CancellationToken token)
        {
            var stopwatch = Stopwatch.StartNew();
            while (!token.IsCancellationRequested)
            {
                stopwatch.Restart();

                // データ取得
                String messageString;
                bool hasAnomalyData;
                await this._chachLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (this._chacheData == null)
                    {
                        await Task.Delay(this._collectInterval, token);
                        this._chachLock.Release();
                        continue;
                    }
                    messageString = JsonConvert.SerializeObject(this._chacheData);
                    hasAnomalyData = HasAnomalyTemp(this._chacheData.Data);
                }
                finally
                {
                    this._chachLock.Release();
                }

                // D2Cメッセージ生成
                var message = new Message(Encoding.UTF8.GetBytes(messageString));
                message.Properties.Add("anomaly", hasAnomalyData ? "true" : "false");

#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます
                BrightAsync(TimeSpan.FromSeconds(1), token);
#pragma warning restore CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

                // 送信
                await Retry.ExecuteAsync(
                    async () => await this._client.SendEventAsync(message).ConfigureAwait(false),
                    5,
                    TimeSpan.FromSeconds(1),
                    (e) => e is IotHubCommunicationException);
                this._logger.LogInformation($"{DateTime.UtcNow} > Sending message: {messageString}");

                // キャッシュを削除
                await this._chachLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    this._chacheData = null;
                }
                finally
                {
                    this._chachLock.Release();
                }

                // まつ。
                var interval = this._telemetryInterval.Ticks - stopwatch.Elapsed.Ticks;
                await Task.Delay(interval > 0 ? TimeSpan.FromTicks(interval) : TimeSpan.Zero, token);
            }
        }

        private bool HasAnomalyTemp(IReadOnlyList<IReadOnlyList<float>> data)
        {
            foreach(var row in data)
            {
                foreach(var value in row)
                {
                    if (value > this._threshold)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task BrightAsync(TimeSpan time, CancellationToken token)
        {
            this._led.ON();
            await Task.Delay(time, token);
            this._led.OFF();
        }

        private Task<MethodResponse> GetNowThermoDataAsync(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);

            this._logger.LogInformation($"Recieve Message: {data}");

            var thermoValue = this._thermoSensor.ReadThermo();
            var result = new ThermoTelemetryData()
            {
                Timestamp = DateTimeOffset.UtcNow,
                Data = thermoValue
            };

            var messageString = JsonConvert.SerializeObject(result);

            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(messageString), 200));
        }


        private float GetMaxValue(IReadOnlyList<IReadOnlyList<float>> data)
        {
            var flat = data.SelectMany(i => i);
            return flat.Max();
        }
    }
}
