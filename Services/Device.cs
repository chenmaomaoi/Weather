using System.Device.Gpio;
using Iot.Device.Bmxx80;
using System.IO.Ports;
using Iot.Device.Sht3x;
using nanoFramework.Hardware.Esp32;
using System.Device.I2c;
using Iot.Device.Rtc;
using Weather.Services.Interfaces;

namespace Weather.Services
{
    public class Device : ISingletonService
    {
        /// <summary>
        /// BMP280
        /// </summary>
        public Bmp280 bmp280;

        /// <summary>
        /// SHT30
        /// </summary>
        public Sht3x sht30;

        /// <summary>
        /// BLE连接状态
        /// </summary>
        public GpioPin bleState;

        /// <summary>
        /// 蓝牙串口
        /// </summary>
        public SerialPort blePort;

        /// <summary>
        /// DS1307
        /// </summary>
        public Ds1307 DS1307;

        /// <summary>
        /// IIC总线速率
        /// </summary>
        private const I2cBusSpeed busSpeed = I2cBusSpeed.FastModePlus;

        public Device()
        {
            #region IIC
            Configuration.SetPinFunction(GPIOConfigs.IIC_SCL, DeviceFunction.I2C1_CLOCK);
            Configuration.SetPinFunction(GPIOConfigs.IIC_SDA, DeviceFunction.I2C1_DATA);
            #endregion

            #region 串口BLE
            Configuration.SetPinFunction(GPIOConfigs.BLE_RX, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(GPIOConfigs.BLE_TX, DeviceFunction.COM2_TX);
            bleState = new GpioController().OpenPin(GPIOConfigs.BLE_State, PinMode.Input);
            blePort = new SerialPort("COM2", 115200);
            #endregion

            //BMP280
            bmp280 = new Bmp280(
                I2cDevice.Create(
                    new I2cConnectionSettings(1, 0x76, busSpeed)));

            //SHT30
            sht30 = new Sht3x(
                I2cDevice.Create(
                    new I2cConnectionSettings(1, 0x44, busSpeed)));

            //DS1307
            DS1307 = new Ds1307(
                I2cDevice.Create(
                    new I2cConnectionSettings(1, Ds1307.DefaultI2cAddress, busSpeed)));
        }
    }
}
