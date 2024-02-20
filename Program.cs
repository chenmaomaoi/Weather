using System;
using System.Threading;
using nanoFramework.Hosting;
using Weather.Services.Extensions;

namespace Weather
{
    public class Program
    {
        public static IHost host = null;

        public static void Main()
        {
            try
            {
                host ??= Host.CreateDefaultBuilder()
                         .ConfigureServices(services => services.AddServices())
                         .Build();
                host.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
