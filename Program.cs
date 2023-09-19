using nanoFramework.Hosting;
using Weather.Services.Extensions;

namespace Weather
{
    public class Program
    {
        public static IHost host = null;

        public static void Main()
        {
            host ??= Host.CreateDefaultBuilder()
                     .ConfigureServices(services => services.AddServices())
                     .Build();
            host.Run();
        }
    }
}
