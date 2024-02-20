using System;
using System.Threading;
using nanoFramework.Hosting;
using nanoFramework.Json;
using nanoFramework.Networking;
using Weather.Data;

namespace Weather.Services
{
    public class ApplicationService : SchedulerService, IHostedService
    {
        private readonly Device device;
        private readonly SensorReaderService sensorReader;
        private readonly BLEService ble;

        public ApplicationService(Device device, SensorReaderService sensorReader, BLEService ble)
            : base(TimeSpan.FromSeconds(3))
        {
            this.device = device;
            this.sensorReader = sensorReader;
            this.ble = ble;

            ConnectWiFi("m204", "17605214170");
        }

        /// <summary>
        /// 连接WiFi
        /// </summary>
        /// <param name="SSID"></param>
        /// <param name="Password"></param>
        private void ConnectWiFi(string SSID, string Password)
        {
            // Give 60 seconds to the wifi join to happen
            if (!WifiNetworkHelper.ScanAndConnectDhcp(
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

        protected override void ExecuteAsync()
        {
            if (ble.IsConnected)
            {
                //读取传感器
                Dto dto = new Dto()
                {
                    Time = DateTime.UtcNow,
                    SHT30 = sensorReader.SHT30,
                    BMP280 = sensorReader.BMP280
                };
                //发送
                nanoFramework.Runtime.Native.GC.Run(true);
                ble.WriteLine(JsonConvert.SerializeObject(dto));
                nanoFramework.Runtime.Native.GC.Run(true);
            }
            GC.WaitForPendingFinalizers();
        }
    }
}