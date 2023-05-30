﻿using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.IO.Ports;
using System.Threading;
using Iot.Device.Bmxx80;
using Iot.Device.CharacterLcd;
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
        private readonly ButtonService buttonService;
        private readonly LEDBlinkService ledBlink;
        private readonly Lcd1602 lcd1602;
        private readonly Bmp280 bmp280;
        private readonly Sht3x sht30;
        private readonly SerialPort blePort;

        public ApplicationService(Device device, ButtonService buttonService, LEDBlinkService ledBlink)
            : base(TimeSpan.FromSeconds(2))
        {
            this.device = device;
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
            //摄氏度字符
            byte[] charCelsius = new byte[8]
            {
                0b10000,
                0b00110,
                0b01001,
                0b01000,
                0b01000,
                0b01001,
                0b00110,
                0b00000
            };
            lcd1602.CreateCustomCharacter(1, charCelsius);

            //BMP280
            I2cDevice i2cBmp280Device = I2cDevice.Create(new I2cConnectionSettings(1, 0x76, I2cBusSpeed.FastModePlus));
            bmp280 = new Bmp280(i2cBmp280Device);

            //SHT30
            I2cDevice i2cSht3xDevice = I2cDevice.Create(new I2cConnectionSettings(1, 0x44, I2cBusSpeed.FastModePlus));
            sht30 = new Sht3x(i2cSht3xDevice);

            //蓝牙串口
            blePort = new SerialPort("COM2", 115200);
        }

        public override void Start()
        {
            base.Start();

            lcd1602.BacklightOn = true;
            lcd1602.DisplayOn = true;

            bmp280.TemperatureSampling = Sampling.UltraHighResolution;
            bmp280.PressureSampling = Sampling.UltraHighResolution;
            bmp280.FilterMode = Iot.Device.Bmxx80.FilteringMode.Bmx280FilteringMode.X2;

            sht30.Resolution = Resolution.High;
            sht30.Heater = true;

            blePort.Open();
        }

        public override void Stop()
        {
            base.Stop();

            ledBlink.Dark();

            //关闭显示器
            lcd1602.Clear();
            lcd1602.BacklightOn = false;
            lcd1602.DisplayOn = false;

            bmp280.TemperatureSampling = Sampling.Skipped;
            bmp280.PressureSampling = Sampling.Skipped;
            bmp280.FilterMode = Iot.Device.Bmxx80.FilteringMode.Bmx280FilteringMode.Off;

            sht30.Resolution = Resolution.Low;
            sht30.Heater = false;

            blePort.Close();
        }

        protected override void ExecuteAsync()
        {
            ledBlink.Bright();
            Thread.Sleep(10);
            ledBlink.Dark();

            DataDto data = new DataDto()
            {
                BMP280 = bmp280.Read(),
                SHT30 = new DataDto.SHT30Result()
                {
                    Temperature = sht30.Temperature,
                    RelativeHumidity = sht30.Humidity
                }
            };

            sht30.Heater = true;

            lcd1602.Home();
            lcd1602.Write($"T:{data.SHT30.Temperature.DegreesCelsius.ToString("F1")}\x1 ");
            lcd1602.Write($"RH:{data.SHT30.RelativeHumidity.Percent.ToString("F1")}% ");
            lcd1602.SetCursorPosition(0, 1);
            lcd1602.Write($"{data.BMP280.Pressure.Hectopascals.ToString("F1")}hPa ");
            lcd1602.Write($"{WeatherHelper.CalculateAltitude(data.BMP280.Pressure, data.BMP280.Temperature).Meters.ToString("F1")}m ");

            //蓝牙已连接，发送数据
            if (device.BLE_State.Read() == PinValue.High)
            {
                blePort.WriteLine(JsonConvert.SerializeObject(data));
            }
        }
    }
}
