using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core
{
    /// <summary>
    /// Add static service resolver to use when dependencies injection is not available
    /// Ref https://www.davidezoccarato.cloud/resolving-instances-with-asp-net-core-di-in-static-classes/
    /// Ref https://stackoverflow.com/a/55678060/581476
    /// </summary>
    public class ServiceActivator
    {
        internal static IServiceProvider ServiceProvider;

        /// <summary>
        /// Configure ServiceActivator with full serviceProvider
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Configure(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Create a scope where use this ServiceActivator
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IServiceScope GetScope(IServiceProvider serviceProvider = null)
        {
            var provider = serviceProvider ?? ServiceProvider;
            return provider?
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
        }

        public static T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        public static IEnumerable<T> GetServices<T>()
        {
            return ServiceProvider.GetServices<T>();
        }

        public static object GetService(Type type)
        {
            return ServiceProvider.GetService(type);
        }

        public static IEnumerable<object> GetServices(Type type)
        {
            return ServiceProvider.GetServices(type);
        }
    }
}