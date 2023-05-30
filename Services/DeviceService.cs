using System.Device.Gpio;
using Iot.Device.Button;
using nanoFramework.Hardware.Esp32;
using Weather.Services.Extensions.DependencyAttribute;

namespace Weather.Services
{
    [SingletonDependency]
    public class DeviceService
    {
        /// <summary>
        /// 板载指示灯
        /// </summary>
        public GpioPin Light;

        /// <summary>
        /// 板载按钮
        /// </summary>
        public GpioButton Button;

        public DeviceService()
        {
            Light = new GpioController().OpenPin(GPIOConfigs.OnBoardLigth);

            Button = new GpioButton(GPIOConfigs.OnBoardButton);

            Configuration.SetPinFunction(GPIOConfigs.IIC_SCL, DeviceFunction.I2C1_CLOCK);
            Configuration.SetPinFunction(GPIOConfigs.IIC_SDA, DeviceFunction.I2C1_DATA);

        }
    }
}
