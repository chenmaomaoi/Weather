using nanoFramework.Hardware.Esp32;

namespace Weather
{
    public static class GPIOConfigs
    {
        /// <summary>
        /// 板载按钮
        /// </summary>
        public const int OnBoardButton = Gpio.IO00;

        /// <summary>
        /// 板载LED灯
        /// </summary>
        public const int OnBoardLigth = Gpio.IO02;

        #region IIC
        public const int IIC_SCL = Gpio.IO16;
        public const int IIC_SDA = Gpio.IO17;
        #endregion

        #region BLE
        public const int BLE_RX = Gpio.IO27;
        public const int BLE_TX = Gpio.IO26;
        public const int BLE_State = Gpio.IO04;
        #endregion

        /// <summary>
        /// 设置原点按钮
        /// </summary>
        public const int SetLandmark = Gpio.IO00;
    }
}
