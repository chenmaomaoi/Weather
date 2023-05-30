using System;
using System.Device.I2c;
using System.Threading;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.ReadResult;
using Iot.Device.CharacterLcd;
using Iot.Device.Sht3x;
using nanoFramework.Hosting;
using Weather.Services.Extensions.DependencyAttribute;

namespace Weather.Services
{
    [HostedDependency]
    public class ApplicationService : SchedulerService
    {
        private readonly ButtonService buttonService;
        private readonly LEDBlinkService ledBlink;
        private readonly Lcd1602 lcd1602;
        private readonly Bmp280 bmp280;
        private readonly Sht3x sht30;

        public ApplicationService(ButtonService buttonService, LEDBlinkService ledBlink)
            : base(TimeSpan.FromSeconds(2))
        {
            this.buttonService = buttonService;
            this.ledBlink = ledBlink;

            //LCD1602
            I2cDevice i2cLcdDevice = I2cDevice.Create(new I2cConnectionSettings(1, 0x27, I2cBusSpeed.FastModePlus));
            LcdInterface lcdInterface = LcdInterface.CreateI2c(i2cLcdDevice, false);
            lcd1602 = new Lcd1602(lcdInterface)
            {
                UnderlineCursorVisible = false,
                BacklightOn = true
            };

            //BMP280
            I2cDevice i2cBmp280Device = I2cDevice.Create(new I2cConnectionSettings(1, 0x76, I2cBusSpeed.FastModePlus));
            bmp280 = new Bmp280(i2cBmp280Device)
            {
                TemperatureSampling = Sampling.UltraHighResolution,
                PressureSampling = Sampling.UltraHighResolution
            };

            //SHT30
            I2cDevice i2cSht3xDevice = I2cDevice.Create(new I2cConnectionSettings(1, 0x44, I2cBusSpeed.FastModePlus));
            sht30 = new Sht3x(i2cSht3xDevice);
        }

        public override void Start()
        {
            base.Start();

            lcd1602.BacklightOn = true;
            lcd1602.DisplayOn = true;
        }

        public override void Stop()
        {
            base.Stop();

            //todo:低功耗模式

            //关闭闪烁
            ledBlink.Dark();

            //关闭显示器
            lcd1602.Clear();
            lcd1602.BacklightOn = false;
            lcd1602.DisplayOn = false;
        }

        protected override void ExecuteAsync()
        {
            ledBlink.Bright();
            Thread.Sleep(10);
            ledBlink.Dark();

            //读取温湿度气压，显示，串口发送数据（按需）

            Bmp280ReadResult readResult = bmp280.Read();
            lcd1602.Home();
            lcd1602.Write($"T:{sht30.Temperature.DegreesCelsius.ToString("F1")}C ");
            lcd1602.Write($"RH:{sht30.Humidity.Percent.ToString("F1")}% ");
            lcd1602.SetCursorPosition(0, 1);
            lcd1602.Write($"P:{readResult.Pressure.Hectopascals.ToString("F1")}hPa ");
        }
    }
}
