using System.Device.Gpio;
using Iot.Device.Button;
using nanoFramework.Hardware.Esp32;
using Weather.Services.Extensions.DependencyAttribute;

namespace Weather.Services
{
    [SingletonDependency]
    public class Device
    {
        /// <summary>
        /// 板载指示灯
        /// </summary>
        public GpioPin ledLight;

        /// <summary>
        /// 板载按钮
        /// </summary>
        //public GpioButton btnButton;

        /// <summary>
        /// BLE连接状态
        /// </summary>
        public GpioPin BLE_State;

        /// <summary>
        /// 设置原点按钮
        /// </summary>
        public GpioButton btnSetLandmark;

        public Device()
        {
            ledLight = new GpioController().OpenPin(GPIOConfigs.OnBoardLigth, PinMode.Output);

            //btnButton = new GpioButton(GPIOConfigs.OnBoardButton);

            #region IIC
            Configuration.SetPinFunction(GPIOConfigs.IIC_SCL, DeviceFunction.I2C1_CLOCK);
            Configuration.SetPinFunction(GPIOConfigs.IIC_SDA, DeviceFunction.I2C1_DATA);
            #endregion

            #region BLE
            Configuration.SetPinFunction(GPIOConfigs.BLE_RX, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(GPIOConfigs.BLE_TX, DeviceFunction.COM2_TX);
            BLE_State = new GpioController().OpenPin(GPIOConfigs.BLE_State, PinMode.Input);
            #endregion

            btnSetLandmark = new GpioButton(GPIOConfigs.SetLandmark);
        }
    }
}
