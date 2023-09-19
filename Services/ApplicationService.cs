using System;
using System.Threading;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.ReadResult;
using Iot.Device.Common;
using Iot.Device.Sht3x;
using nanoFramework.Hosting;
using nanoFramework.Json;
using nanoFramework.Networking;
using nanoFramework.Runtime.Native;

namespace Weather.Services
{
    public class ApplicationService : SchedulerService, IHostedService
    {
        private readonly Device device;

        public ApplicationService(Device device) : base(TimeSpan.FromSeconds(1))
        {
            this.device = device;

            device.bleState.ButtonDown += (object sender, EventArgs e) => this.Start();
            device.bleState.ButtonUp += (object sender, EventArgs e) => this.Stop();
            this.Stop();

            Rtc.SetSystemTime(device.DS1307.DateTime);
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

        public override void Start()
        {
            base.Start();

            device.bmp280.TemperatureSampling = Sampling.UltraHighResolution;
            device.bmp280.PressureSampling = Sampling.UltraHighResolution;
            device.bmp280.FilterMode = Iot.Device.Bmxx80.FilteringMode.Bmx280FilteringMode.X4;

            device.sht30.Resolution = Resolution.High;
            device.sht30.Heater = true;

            device.blePort.Open();
        }

        public override void Stop()
        {
            base.Stop();

            device.bmp280.TemperatureSampling = Sampling.UltraLowPower;
            device.bmp280.PressureSampling = Sampling.UltraLowPower;

            device.sht30.Resolution = Resolution.Low;
            device.sht30.Heater = false;

            device.blePort.Close();
        }

        protected override void ExecuteAsync()
        {
            //蓝牙已连接，发送数据
            Bmp280ReadResult bmp280result = device.bmp280.Read();
            DataDto data = new()
            {
                Time = DateTime.UtcNow,
                BMP280 = new()
                {
                    Temperature = (float)bmp280result.Temperature.DegreesCelsius,
                    Pressure = (float)bmp280result.Pressure.Hectopascals,
                    Height = (float)WeatherHelper.CalculateAltitude(bmp280result.Pressure, bmp280result.Temperature).Meters
                },
                SHT30 = new DataDto.SHT30Result()
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