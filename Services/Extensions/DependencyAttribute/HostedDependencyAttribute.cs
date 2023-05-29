using System;

namespace Weather.Services.Extensions.DependencyAttribute
{
    /// <summary>
    /// 需要继承IHostedService，随Host启动和停止
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class HostedDependencyAttribute : Attribute
    {
    }
}
