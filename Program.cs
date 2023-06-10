using nanoFramework.Hardware.Esp32;
using nanoFramework.Hosting;
using Weather.Services.Extensions;

namespace Weather
{
    public class Program
    {
        public static IHost host;

        public static void Main()
        {
            Sleep.EnableWakeupByPin(Sleep.WakeupGpioPin.Pin0, 0);

            host ??= Host.CreateDefaultBuilder()
                     .ConfigureServices(services => services.AddServices())
                     .Build();
            host.Run();
        }
    }
}
