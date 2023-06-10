using System;
using Iot.Device.Button;
using Weather.Services.Extensions.DependencyAttribute;

namespace Weather.Services
{
    /// <summary>
    /// 板载按钮服务
    /// </summary>
    //[SingletonDependency]
    public class ButtonService
    {
        private readonly GpioButton button;
        private bool isEnable;

        public ButtonService(Device device)
        {
            //button = device.btnButton;
            isEnable = true;
            button.Press += button_Press;
        }

        private void button_Press(object sender, EventArgs e)
        {
            isEnable = !isEnable;
            if (isEnable)
            {
                Program.host.Start();
            }
            else
            {
                Program.host.Stop();
            }
        }
    }
}
