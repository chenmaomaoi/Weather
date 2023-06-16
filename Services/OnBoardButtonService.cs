using System;
using System.Threading;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.ReadResult;
using Iot.Device.Button;
using Iot.Device.Common;
using nanoFramework.Hardware.Esp32;
using Weather.Services.Extensions.DependencyAttribute;

namespace Weather.Services
{
    /// <summary>
    /// 长按休眠；点击退出休眠 设置当前高度为原点；todo:双击重置高度
    /// </summary>
    [SingletonDependency]
    public class OnBoardButtonService
    {
        private readonly GpioButton btnOnBoard;
        private readonly Bmp280 bmp280;

        private readonly LEDBlinkService blinkService;

        private bool isHolding = false;

        public OnBoardButtonService(Device device, LEDBlinkService blinkService)
        {
            this.btnOnBoard = device.btnOnBoard;
            this.bmp280 = device.bmp280;

            this.blinkService = blinkService;

            btnOnBoard.Press += BtnOnBoard_Press;
            btnOnBoard.Holding += BtnOnBoard_Holding;
            btnOnBoard.DoublePress += BtnOnBoard_DoublePress;
        }

        private void BtnOnBoard_Press(object sender, EventArgs e)
        {
            blinkService.Bright();
            Thread.Sleep(50);
            blinkService.Dark();

            if (isHolding)
            {
                return;
            }

            if (Program.IsRunning == false)
            {
                Program.IsRunning = true;
                Program.host.Start();

                return;
            }

            Bmp280ReadResult bmp280result = bmp280.Read();
            Settings.Hight = WeatherHelper.CalculateAltitude(
                bmp280result.Pressure,
                bmp280result.Temperature)
                .Meters;
        }

        private void BtnOnBoard_DoublePress(object sender, EventArgs e)
        {
            Settings.Hight = 0;
        }

        private void BtnOnBoard_Holding(object sender, Iot.Device.Button.ButtonHoldingEventArgs e)
        {
            isHolding = true;
            Program.host.Stop();

            new Timer((o) =>
            {
                isHolding = false;
                Sleep.StartLightSleep();
            }, new object(), TimeSpan.FromMilliseconds(50), TimeSpan.Zero);
        }
    }
}
