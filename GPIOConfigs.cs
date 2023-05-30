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

        public const int IIC_SCL = Gpio.IO16;

        public const int IIC_SDA = Gpio.IO17;
    }
}
