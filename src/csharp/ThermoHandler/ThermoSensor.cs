using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace ThermoHandler
{
    /// <summary>
    ///     サーモセンサー
    /// </summary>
    public class ThermoSensor : IDisposable
    {
        private static readonly byte PCTL = 0x00;
        private static readonly byte RST = 0x01;
        private static readonly byte FPSC = 0x02;
        private static readonly byte INTC = 0x03;
        private static readonly byte STAT = 0x04;
        private static readonly byte SCLR = 0x05;
        private static readonly byte AVE = 0x07;
        private static readonly byte INTHL = 0x08;
        private static readonly byte TTHL = 0x0E;
        private static readonly byte INT0 = 0x10;
        private static readonly byte T01L = 0x80;

        private static readonly Object LockObj = new object();

        private readonly ILogger _logger;
        private readonly int _i2cMaxRetryCount;
        private readonly TimeSpan _i2cRetryInterval;
         
        private I2cDevice _device;

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/></param>
        /// <param name="i2cMaxRetryCount">リトライする最大の回数</param>
        /// <param name="i2cRetryInterval">リトライ間隔</param>
        public ThermoSensor(ILoggerFactory loggerFactory, int? i2cMaxRetryCount = null, TimeSpan? i2cRetryInterval = null)
        {
            this._logger = loggerFactory.CreateLogger<ThermoSensor>();
            this._i2cMaxRetryCount = i2cMaxRetryCount ?? 10;
            this._i2cRetryInterval = i2cRetryInterval ?? TimeSpan.FromMilliseconds(100);
        }

        /// <summary>
        ///     I2C サーモセンサーを起動します。
        /// </summary>
        /// <param name="busId">サーモセンサーのバスID。</param>
        /// <param name="i2cDeviceAddress">サーモセンサーのデバイスアドレス。</param>
        public void Initialize(int busId, int i2cDeviceAddress)
        {
            var settings = new I2cConnectionSettings(busId, i2cDeviceAddress);
            this._device = I2cDevice.Create(settings);

            this.WriteWithRetry(stackalloc byte[] { FPSC, 0x00 });
            this.WriteWithRetry(stackalloc byte[] { INTC, 0x00 });

            Thread.Sleep(1000);

            Span<byte> readBuf = stackalloc byte[2];
            this.WriteReadWithRetry(stackalloc byte[] { TTHL }, readBuf);

            this._logger.LogInformation($"sensor temperature: {(readBuf[1] * 256 + readBuf[0]) * 0.0625}");
            this._logger.LogInformation($"Thermo sensor (address: 0x{i2cDeviceAddress:X}) is Initialized.");
        }

        /// <summary>
        ///     サーモデータを読み取ります。<see cref="Initialize(int, int)"/> が呼ばれている必要があります。
        /// </summary>
        /// <returns>読みとっとた値。8x8のジャグ配列。</returns>
        public IReadOnlyList<IReadOnlyList<float>> ReadThermo()
        {
            // 8 x 8 になるはず。
            var thermoMatrix = new List<List<float>>(8);

            // Wire library cannnot contain more than 32 bytes in bufffer
            // 2byte per one data
            // 2 byte * 16 data * 4 times
            Span<byte> sensorData = stackalloc byte[32];
            for (int i = 0; i < 4; i++)
            {
                var retryFromInvalidValueFlag = true;
                while (retryFromInvalidValueFlag)
                {
                    // read each 32 bytes
                    this.WriteReadWithRetry(stackalloc byte[] { (byte)(T01L + i * 0x20) }, sensorData);

                    var count = 1;
                    var row = new List<float>(16);
                    try
                    {
                        for (int l = 0; l < 16; l++)
                        {
                            row.Add(this.Decode(sensorData, l));
                        }
                    }
                    catch (InvalidDataException e)
                    {
                        this._logger.LogError(e.ToString());
                        // リトライ
                        if (count > this._i2cMaxRetryCount)
                        {
                            throw;
                        }

                        Thread.Sleep(this._i2cRetryInterval);
                        count++;
                        continue;
                    }

                    thermoMatrix.Add(row.GetRange(0, 8));
                    thermoMatrix.Add(row.GetRange(8, 8));
                    retryFromInvalidValueFlag = false;
                    break;
                }
            }

            return thermoMatrix;
        }

        private float Decode(Span<byte> sensorData, int columnNumber )
        {
            ushort temporaryData = (ushort)((sensorData[columnNumber * 2 + 1] * 256) + sensorData[columnNumber * 2]);
            float temperature;
            if (temporaryData > 0x200)
            {
                temperature = (float)((-temporaryData + 0xfff) * -0.25);
            }
            else
            {
                temperature = (float)(temporaryData * 0.25);
            }

            if (temperature < 0 || 80 < temperature)
            {
                throw new InvalidDataException($"不正な検出値 {temperature}");
            }

            return temperature;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this._device != null)
            {
                try
                {
                    this._device.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // nop
                }
            }
        }

        private void WriteWithRetry(ReadOnlySpan<byte> writeBuffer)
        {
            var count = 1;
            while(count <= this._i2cMaxRetryCount)
            {
                try
                {
                    lock (LockObj)
                    {
                        this._device.Write(writeBuffer);
                    }
                    break;
                }
                catch (IOException e)
                {
                    if (count > this._i2cMaxRetryCount)
                    {
                        throw;
                    }
                    // this._logger.LogDebug($"Retry because thrown a exception. e: {e}");
                    Thread.Sleep(this._i2cRetryInterval);
                    count++;
                }
            }
        }

        private void WriteReadWithRetry(ReadOnlySpan<byte> writeBuffer, Span<byte> readBuffer)
        {
            var count = 1;
            while (count <= this._i2cMaxRetryCount)
            {
                try
                {
                    lock (LockObj)
                    {
                        this._device.WriteRead(writeBuffer, readBuffer);
                    }
                    break;
                }
                catch (IOException e)
                {
                    if (count > this._i2cMaxRetryCount)
                    {
                        throw;
                    }
                    // this._logger.LogDebug($"Retry because thrown a exception. e: {e}");
                    Thread.Sleep(this._i2cRetryInterval);
                    count++;
                }
            }
        }
    }
}
