using System;
using nanoFramework.Hosting;
using Weather.Services.Extensions;

namespace Weather
{
    public class Program
    {
        public static IHost host;

        public static void Main()
        {
            try
            {
                host = Host.CreateDefaultBuilder()
                     .ConfigureServices(services => services.AddServices())
                     .Build();
                host.Run();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }
    }
}
