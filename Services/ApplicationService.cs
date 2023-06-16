using System;
using System.Device.Gpio;
using System.Threading;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.ReadResult;
using Iot.Device.Common;
using Iot.Device.Sht3x;
using nanoFramework.Hosting;
using nanoFramework.Json;
using Weather.Services.Extensions.DependencyAttribute;

namespace Weather.Services
{
    [HostedDependency]
    public class ApplicationService : SchedulerService
    {
        private readonly Device device;

        private readonly LEDBlinkService blinkService;
        private readonly OnBoardButtonService onBoardButtonService;

        public ApplicationService(Device device, LEDBlinkService blinkService, OnBoardButtonService onBoardButtonService)
            : base(TimeSpan.FromSeconds(1))
        {
            this.device = device;

            this.blinkService = blinkService;
            this.onBoardButtonService = onBoardButtonService;
        }

        public override void Start()
        {
            base.Start();

            device.lcd1602.BacklightOn = false;
            device.lcd1602.DisplayOn = true;
            device.lcd1602.Clear();

            device.bmp280.TemperatureSampling = Sampling.UltraHighResolution;
            device.bmp280.PressureSampling = Sampling.UltraHighResolution;
            device.bmp280.FilterMode = Iot.Device.Bmxx80.FilteringMode.Bmx280FilteringMode.X4;

            device.sht30.Resolution = Resolution.High;
            device.sht30.Heater = true;

            device.blePort.Open();

            Program.IsRunning = true;
        }

        public override void Stop()
        {
            blinkService.Bright();
            Thread.Sleep(1000);
            blinkService.StopBlink();
            base.Stop();

            Program.IsRunning = false;

            //关闭显示器
            //lcd1602.BacklightOn = false;
            device.lcd1602.DisplayOn = false;

            device.bmp280.TemperatureSampling = Sampling.UltraLowPower;
            device.bmp280.PressureSampling = Sampling.UltraLowPower;

            device.sht30.Resolution = Resolution.Low;
            device.sht30.Heater = false;

            device.blePort.Close();

            blinkService.Dark();
        }

        protected override void ExecuteAsync()
        {
            blinkService.Bright();
            Thread.Sleep(0);
            blinkService.Dark();

            Bmp280ReadResult bmp280result = device.bmp280.Read();

            DataDto data = new()
            {
                BMP280 = new()
                {
                    Temperature = (float)bmp280result.Temperature.DegreesCelsius,
                    Pressure = (float)bmp280result.Pressure.Hectopascals
                },
                SHT30 = new DataDto.SHT30Result()
                {
                    Temperature = (float)device.sht30.Temperature.DegreesCelsius,
                    RelativeHumidity = (float)device.sht30.Humidity.Percent
                }
            };

            device.sht30.Heater = true;

            device.lcd1602.Home();
            device.lcd1602.Write($"T:{data.BMP280.Temperature.ToString("F1")}\x1 ");
            device.lcd1602.Write($"RH:{data.SHT30.RelativeHumidity.ToString("F1")}% ");
            device.lcd1602.SetCursorPosition(0, 1);
            device.lcd1602.Write($"{data.BMP280.Pressure.ToString("F1")}hPa ");

            double hight = WeatherHelper.CalculateAltitude(bmp280result.Pressure, bmp280result.Temperature).Meters;

            device.lcd1602.Write($"{(hight - Settings.Hight).ToString("F1")}m  ");

            //蓝牙已连接，发送数据
            if (device.bleState.Read() == PinValue.High)
            {
                device.blePort.WriteLine(JsonConvert.SerializeObject(data));
            }

            GC.WaitForPendingFinalizers();
        }
    }
}
