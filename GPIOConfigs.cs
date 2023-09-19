using nanoFramework.Hardware.Esp32;

namespace Weather
{
    public static class GPIOConfigs
    {
        #region IIC
        public const int IIC_SCL = Gpio.IO16;
        public const int IIC_SDA = Gpio.IO17;
        #endregion

        #region BLE
        public const int BLE_RX = Gpio.IO10;
        public const int BLE_TX = Gpio.IO11;
        public const int BLE_State = Gpio.IO04;
        #endregion
    }
}
