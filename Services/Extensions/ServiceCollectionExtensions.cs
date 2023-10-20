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
                if (!type.IsClass)
                {
                    continue;
                }

                foreach (Type item in type.GetInterfaces())
                {
                    bool flag = false;
                    switch (item.Name)
                    {
                        case nameof(IHostedService):
                            services.AddHostedService(type);
                            flag = true;
                            break;
                        case nameof(ISingletonService):
                            services.AddSingleton(type);
                            flag = true;
                            break;
                        case nameof(ITransientService):
                            services.AddTransient(type);
                            flag = true;
                            break;
                        default:
                            break;
                    }

                    if (flag)
                    {
                        break;
                    }
                }
            }
            return services;
        }
    }
}
