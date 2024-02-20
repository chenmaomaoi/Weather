using System.Device.I2c;
using nanoFramework.Hardware.Esp32;

namespace Weather
{
    public static class Configs
    {
        #region IIC
        public const int IIC_SCL = Gpio.IO16;
        public const int IIC_SDA = Gpio.IO17;

        /// <summary>
        /// IIC总线速率
        /// </summary>
        public const I2cBusSpeed I2cSpeed = I2cBusSpeed.FastModePlus;
        #endregion

        #region BLE
        public const int BLE_RX = Gpio.IO10;
        public const int BLE_TX = Gpio.IO11;
        public const int BLE_State = Gpio.IO04;
        public const string BLE_PortName = "COM2";
        public const int BLE_BaudRate = 115200;
        #endregion
    }
}
