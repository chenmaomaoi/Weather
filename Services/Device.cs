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
        public GpioPin Light;

        /// <summary>
        /// 板载按钮
        /// </summary>
        public GpioButton Button;

        /// <summary>
        /// BLE连接状态
        /// </summary>
        public GpioPin BLE_State;

        public Device()
        {
            Light = new GpioController().OpenPin(GPIOConfigs.OnBoardLigth, PinMode.Output);

            Button = new GpioButton(GPIOConfigs.OnBoardButton);

            #region IIC
            Configuration.SetPinFunction(GPIOConfigs.IIC_SCL, DeviceFunction.I2C1_CLOCK);
            Configuration.SetPinFunction(GPIOConfigs.IIC_SDA, DeviceFunction.I2C1_DATA);
            #endregion

            #region BLE
            Configuration.SetPinFunction(GPIOConfigs.BLE_RX, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(GPIOConfigs.BLE_TX, DeviceFunction.COM2_TX);
            BLE_State = new GpioController().OpenPin(GPIOConfigs.BLE_State, PinMode.Input);
            #endregion
        }
    }
}
