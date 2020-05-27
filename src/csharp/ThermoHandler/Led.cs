using System;
using System.Device.Gpio;
using Microsoft.Extensions.Logging;

namespace ThermoHandler
{
    /// <summary>
    ///     LED
    /// </summary>
    public class Led : IDisposable
    {
        private readonly ILogger _logger;
        private GpioController _led;
        private int _pinNumber;

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/></param>
        public Led(ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<Led>();
        }

        /// <summary>
        ///     LEDを起動します。
        /// </summary>
        /// <param name="pinNumber"></param>
        public void Initialize(int pinNumber)
        {
            this._led = new GpioController();
            this._pinNumber = pinNumber;
            this._led.OpenPin(this._pinNumber, PinMode.Output);
            this._logger.LogInformation($"LED (pin: {this._pinNumber}) is Initialized.");
        }

        /// <summary>
        ///     LEDを点灯させます。
        /// </summary>
        public void ON()
        {
            this._led.Write(this._pinNumber, PinValue.High);
            this._logger.LogDebug($"LED (pin: {this._pinNumber}) turn on.");
        }

        /// <summary>
        ///     LEDを消灯させます。
        /// </summary>
        public void OFF()
        {
            this._led.Write(this._pinNumber, PinValue.Low);
            this._logger.LogDebug($"LED (pin: {this._pinNumber}) turn off.");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this._led != null)
            {
                try
                {
                    this._led.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // nop
                }
            }
        }
    }
}
