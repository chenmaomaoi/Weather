using nanoFramework.Hardware.Esp32;
using nanoFramework.Hosting;
using Weather.Services.Extensions;

namespace Weather
{
    public class Program
    {
        public static IHost host = null;

        public static bool IsRunning = false;

        public static void Main()
        {
            Sleep.EnableWakeupByPin(GPIOConfigs.pinWakeup, 0);

            host ??= Host.CreateDefaultBuilder()
                     .ConfigureServices(services => services.AddServices())
                     .Build();
            host.Run();
        }
    }
}
