using System.Device.Gpio;
using Iot.Device.Bmxx80;
using System.IO.Ports;
using Iot.Device.Button;
using Iot.Device.CharacterLcd;
using Iot.Device.Sht3x;
using nanoFramework.Hardware.Esp32;
using Weather.Services.Extensions.DependencyAttribute;
using System.Device.I2c;

namespace Weather.Services
{
    [SingletonDependency]
    public class Device
    {
        /// <summary>
        /// 板载指示灯
        /// </summary>
        public GpioPin ledOnBoardLight;

        /// <summary>
        /// 睡眠按钮
        /// </summary>
        public GpioButton btnSleep;

        /// <summary>
        /// 设置原点按钮
        /// </summary>
        public GpioButton btnSetLandmark;

        /// <summary>
        /// LCD1602
        /// </summary>
        public Lcd1602 lcd1602;

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

        public Device()
        {
            ledOnBoardLight = new GpioController().OpenPin(GPIOConfigs.ledOnBoardLigth, PinMode.Output);

            btnSleep = new GpioButton(GPIOConfigs.btnSleep);

            #region IIC
            Configuration.SetPinFunction(GPIOConfigs.IIC_SCL, DeviceFunction.I2C1_CLOCK);
            Configuration.SetPinFunction(GPIOConfigs.IIC_SDA, DeviceFunction.I2C1_DATA);
            #endregion

            #region BLE
            Configuration.SetPinFunction(GPIOConfigs.BLE_RX, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(GPIOConfigs.BLE_TX, DeviceFunction.COM2_TX);
            bleState = new GpioController().OpenPin(GPIOConfigs.BLE_State, PinMode.Input);
            #endregion

            btnSetLandmark = new GpioButton(GPIOConfigs.btnSetLandmark);

            //BMP280
            I2cDevice i2cBmp280Device = I2cDevice.Create(new I2cConnectionSettings(1, 0x76, I2cBusSpeed.FastModePlus));
            bmp280 = new Bmp280(i2cBmp280Device);

            //SHT30
            I2cDevice i2cSht3xDevice = I2cDevice.Create(new I2cConnectionSettings(1, 0x44, I2cBusSpeed.FastModePlus));
            sht30 = new Sht3x(i2cSht3xDevice);

            //LCD1602
            I2cDevice i2cLcdDevice = I2cDevice.Create(new I2cConnectionSettings(1, 0x27, I2cBusSpeed.FastModePlus));
            LcdInterface lcdInterface = LcdInterface.CreateI2c(i2cLcdDevice, false);
            lcd1602 = new Lcd1602(lcdInterface)
            {
                UnderlineCursorVisible = false,
                BacklightOn = false
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

            //蓝牙串口
            blePort = new SerialPort("COM2", 115200);

        }
    }
}
