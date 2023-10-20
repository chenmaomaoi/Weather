using System;
using System.Device.Gpio;
using System.Threading;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.ReadResult;
using Iot.Device.Common;
using Iot.Device.Sht3x;
using nanoFramework.Hosting;
using nanoFramework.Json;
using nanoFramework.Networking;
using nanoFramework.Runtime.Native;
using static Weather.Services.DataDto;

namespace Weather.Services
{
    public class ApplicationService : SchedulerService, IHostedService
    {
        private readonly Device device;

        public ApplicationService(Device device) : base(TimeSpan.FromSeconds(1))
        {
            this.device = device;

            Rtc.SetSystemTime(device.DS1307.DateTime);

            base.Start();
        }

        /// <summary>
        /// 连接WiFi
        /// </summary>
        /// <param name="SSID"></param>
        /// <param name="Password"></param>
        private void ConnectWiFi(string SSID, string Password)
        {
            SSID = "B321";
            Password = "321321321";
            // Give 60 seconds to the wifi join to happen
            if (!WifiNetworkHelper.ConnectDhcp(
                SSID,
                Password,
                requiresDateTime: true,
                token: new CancellationTokenSource(60000).Token))
            {
                // Something went wrong, you can get details with the ConnectionError property:
                Console.WriteLine($"Can't connect to the network, error: {WifiNetworkHelper.Status}");
                if (WifiNetworkHelper.HelperException != null)
                {
                    Console.WriteLine($"ex: {WifiNetworkHelper.HelperException}");
                }
            }
            else
            {
                device.DS1307.DateTime = DateTime.UtcNow;
            }
        }

        private void start()
        {
            if (device.blePort.IsOpen)
            {
                return;
            }

            device.bmp280.TemperatureSampling = Sampling.UltraHighResolution;
            device.bmp280.PressureSampling = Sampling.UltraHighResolution;
            device.bmp280.FilterMode = Iot.Device.Bmxx80.FilteringMode.Bmx280FilteringMode.X4;

            device.sht30.Resolution = Resolution.High;
            device.sht30.Heater = true;

            device.blePort.Open();
        }

        private void stop()
        {
            if (!device.blePort.IsOpen)
            {
                return;
            }

            device.bmp280.TemperatureSampling = Sampling.UltraLowPower;
            device.bmp280.PressureSampling = Sampling.UltraLowPower;

            device.sht30.Resolution = Resolution.Low;
            device.sht30.Heater = false;

            device.blePort.Close();
        }

        protected override void ExecuteAsync()
        {
            if (device.bleState.Read() == PinValue.Low)
            {
                this.stop();
                return;
            }

            this.start();
            //蓝牙已连接，发送数据
            Bmp280ReadResult bmp280result = device.bmp280.Read();
            DataDto data = new DataDto()
            {
                Time = DateTime.UtcNow,
                BMP280 = new BMP280Result()
                {
                    Temperature = (float)bmp280result.Temperature.DegreesCelsius,
                    Pressure = (float)bmp280result.Pressure.Hectopascals,
                    Height = (float)WeatherHelper.CalculateAltitude(bmp280result.Pressure, bmp280result.Temperature).Meters
                },
                SHT30 = new SHT30Result()
                {
                    Temperature = (float)device.sht30.Temperature.DegreesCelsius,
                    RelativeHumidity = (float)device.sht30.Humidity.Percent
                }
            };

            device.sht30.Heater = true;
            device.blePort.WriteLine(JsonConvert.SerializeObject(data));

            System.GC.WaitForPendingFinalizers();
        }
    }
}