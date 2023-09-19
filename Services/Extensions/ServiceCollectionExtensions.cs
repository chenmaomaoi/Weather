using System;
using nanoFramework.DependencyInjection;
using nanoFramework.Hosting;
using System.Reflection;
using Weather.Services.Interfaces;

namespace Weather.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException();

            //获取所有类型
            Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in allTypes)
            {
                switch (type)
                {
                    case IHostedService:
                        services.AddHostedService(type);
                        break;
                    case ISingletonService:
                        services.AddSingleton(type);
                        break;
                    case ITransientService:
                        services.AddTransient(type);
                        break;
                    default:
                        break;
                }
            }
            return services;
        }
    }
}
