using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ThermoHandler.models;

namespace ThermoHandler
{
    public class Program
    {
        /// <summary>
        ///     エントリーポイント
        /// </summary>
        /// <param name="args"></param>
        static async Task Main(string[] args)
        {
            using (var loggerFactory = LoggerFactory.Create(builder =>
             {
                 builder
                     .AddFilter("Microsoft", LogLevel.Warning)
                     .AddFilter("System", LogLevel.Warning)
                     .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                     .AddConsole(c =>
                     {
                         c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                     });
             }))
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogInformation("開始！！");

                //var ledPin = 17;
                //var thermoBusId = 1;
                //var thermoDeviceAddress = 0x68;

                var settingFilePath = Path.Join(Directory.GetCurrentDirectory(), "settings.json");
                var settings = JsonConvert.DeserializeObject<DeviceSettings>(File.ReadAllText(settingFilePath, Encoding.UTF8));

                var device = new ThermoDevice(settings, loggerFactory);

                var cancellationTokenSource = new CancellationTokenSource();
                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
                {
                    logger.LogInformation("終了！！");

                    cancellationTokenSource.Cancel();

                    device.Dispose();
                };


                await device.StertAsync(cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }

        private static void printThermo(IReadOnlyList<IReadOnlyList<float>> matrix, ILogger logger)
        {
            Console.WriteLine("---------------------------------------------");
            foreach(var row in matrix)
            {
                var stirngRow = row.Select(v => v >= 10 ? v.ToString("F1") : "0" + v.ToString("F1"));
                Console.WriteLine(String.Join(" ", stirngRow));

            }
            Console.WriteLine("---------------------------------------------");
        }
    }
}
