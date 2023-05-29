using System;
using nanoFramework.DependencyInjection;
using nanoFramework.Hosting;
using System.Reflection;
using Weather.Services.Extensions.DependencyAttribute;

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
                if (type.IsClass)
                {
                    object[] attributes = type.GetCustomAttributes(false);

                    foreach (object attribute in attributes)
                    {
                        string attr = attribute.ToString();
                        if (attr == typeof(HostedDependencyAttribute).FullName)
                        {
                            services.AddHostedService(type);
                        }
                        else if (attr == typeof(SingletonDependencyAttribute).FullName)
                        {
                            services.AddSingleton(type);
                        }
                        else if (attr == typeof(TransientDependencyAttribute).FullName)
                        {
                            services.AddTransient(type);
                        }
                    }
                }
            }
            return services;
        }
    }
}
