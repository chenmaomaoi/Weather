using System;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hosting;
using Weather.Services.Extensions;

namespace Weather
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                Host.CreateDefaultBuilder()
                    .ConfigureServices(services => services.AddServices())
                    .Build()
                    .Run();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error:");
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
