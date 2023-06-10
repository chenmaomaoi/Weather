using System;
using System.Device.Gpio;
using System.Threading;
using Weather.Services.Extensions.DependencyAttribute;

namespace Weather.Services
{
    /// <summary>
    /// 板载LED闪烁服务
    /// </summary>
    [SingletonDependency]
    public class LEDBlinkService
    {
        /// <summary>
        /// LED灯
        /// </summary>
        private GpioPin Light;

        /// <summary>
        /// 停止信号
        /// </summary>
        private bool signalStop;

        /// <summary>
        /// 控制LED闪烁的线程
        /// </summary>
        private Thread executingThread;

        public LEDBlinkService(Device device)
        {
            Light = device.ledOnBoardLight;
            new GpioController().SetPinMode(Light.PinNumber, PinMode.Output);

            signalStop = false;
        }

        /// <summary>
        /// 闪烁
        /// </summary>
        /// <param name="brigth">亮灯时间</param>
        /// <param name="goOut">灭灯时间</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void StartBlinkAsync(int brigth = 50, int goOut = 950)
        {
            if (brigth < 0) throw new ArgumentOutOfRangeException(nameof(brigth));
            if (goOut < 0) throw new ArgumentOutOfRangeException(nameof(goOut));

            if (executingThread != null)
            {
                StopBlink();
            }
            signalStop = false;

            executingThread = new Thread(() =>
            {
                int th_bright = brigth;
                int th_goOut = goOut;
                while (!signalStop)
                {
                    Bright();
                    Thread.Sleep(brigth);
                    Dark();
                    Thread.Sleep(goOut);
                }
            });
            executingThread.Start();
        }

        /// <summary>
        /// 停止闪烁
        /// </summary>
        public void StopBlink()
        {
            signalStop = true;

            while (executingThread != null && executingThread.ThreadState != ThreadState.Stopped)
            {
                Thread.Sleep(50);
            }
            Dark();
        }

        public void Bright() => Light.Write(PinValue.High);

        public void Dark() => Light.Write(PinValue.Low);
    }
}
