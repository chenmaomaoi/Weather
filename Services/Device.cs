using System.Device.Gpio;
using Iot.Device.Bmxx80;
using System.IO.Ports;
using Iot.Device.Sht3x;
using nanoFramework.Hardware.Esp32;
using System.Device.I2c;
using Iot.Device.Rtc;
using Weather.Services.Interfaces;
using nanoFramework.Runtime.Native;

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

        public Device()
        {
            #region IIC
            Configuration.SetPinFunction(Configs.IIC_SCL, DeviceFunction.I2C1_CLOCK);
            Configuration.SetPinFunction(Configs.IIC_SDA, DeviceFunction.I2C1_DATA);
            #endregion

            #region 串口BLE
            Configuration.SetPinFunction(Configs.BLE_RX, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(Configs.BLE_TX, DeviceFunction.COM2_TX);
            bleState = new GpioController().OpenPin(Configs.BLE_State, PinMode.Input);
            blePort = new SerialPort(Configs.BLE_PortName, Configs.BLE_BaudRate);
            #endregion

            //BMP280
            bmp280 = new Bmp280(
                I2cDevice.Create(
                    new I2cConnectionSettings(1, 0x76, Configs.I2cSpeed)));

            //SHT30
            sht30 = new Sht3x(
                I2cDevice.Create(
                    new I2cConnectionSettings(1, 0x44, Configs.I2cSpeed)));

            //DS1307
            DS1307 = new Ds1307(
                I2cDevice.Create(
                    new I2cConnectionSettings(1, Ds1307.DefaultI2cAddress, Configs.I2cSpeed)));
            Rtc.SetSystemTime(DS1307.DateTime);

        }
    }
}
