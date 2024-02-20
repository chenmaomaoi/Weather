using System;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.ReadResult;
using Iot.Device.Common;
using Iot.Device.Sht3x;
using Weather.Data;
using Weather.Services.Interfaces;

namespace Weather.Services
{
    /// <summary>
    /// 读取传感器参数
    /// </summary>
    public class SensorReaderService : ISingletonService
    {
        private readonly Sht3x sht30;
        private readonly Bmp280 bmp280;

        private bool _enable;
        public bool Enable
        {
            get => _enable;
            set
            {
                //启用和停用传感器
                //未启用禁止get其他数据
                if (value != _enable)
                {
                    if (value)
                    {
                        enable();
                    }
                    else
                    {
                        disable();
                    }
                }
            }
        }

        public SHT30Result SHT30
        {
            get
            {
                checkEnable();
                return new SHT30Result()
                {
                    Temperature = (float)sht30.Temperature.DegreesCelsius,
                    RelativeHumidity = (float)sht30.Humidity.Percent
                };
            }
        }

        public BMP280Result BMP280
        {
            get
            {
                checkEnable();
                Bmp280ReadResult bmp280result = bmp280.Read();
                return new BMP280Result()
                {
                    Temperature = (float)bmp280result.Temperature.DegreesCelsius,
                    Pressure = (float)bmp280result.Pressure.Hectopascals,
                    Height = (float)WeatherHelper.CalculateAltitude(bmp280result.Pressure, bmp280result.Temperature).Meters
                };
            }
        }

        public SensorReaderService(Device device)
        {
            sht30 = device.sht30;
            bmp280 = device.bmp280;
            Enable = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException">传感器服务未启用</exception>
        private void checkEnable()
        {
            if (!Enable)
            {
                throw new InvalidOperationException("传感器服务未启用");
            }
        }

        private void enable()
        {
            bmp280.TemperatureSampling = Sampling.UltraHighResolution;
            bmp280.PressureSampling = Sampling.UltraHighResolution;
            bmp280.FilterMode = Iot.Device.Bmxx80.FilteringMode.Bmx280FilteringMode.X4;

            sht30.Resolution = Resolution.High;
            sht30.Heater = true;

            _enable = true;
        }

        private void disable()
        {
            bmp280.TemperatureSampling = Sampling.UltraLowPower;
            bmp280.PressureSampling = Sampling.UltraLowPower;

            sht30.Resolution = Resolution.Low;
            sht30.Heater = false;

            _enable = false;
        }
    }
}
