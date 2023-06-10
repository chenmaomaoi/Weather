using System;
using Iot.Device.Button;
using Weather.Services.Extensions.DependencyAttribute;

namespace Weather.Services
{
    /// <summary>
    /// 睡眠按钮服务
    /// </summary>
    [SingletonDependency]
    public class SleepButtonService
    {
        private readonly GpioButton btnSleep;

        public SleepButtonService(Device device)
        {
            btnSleep = device.btnSleep;
            btnSleep.Press += BtnSleep_Press;
        }

        private void BtnSleep_Press(object sender, EventArgs e)
        {
            Program.host.Stop();
        }
    }
}
