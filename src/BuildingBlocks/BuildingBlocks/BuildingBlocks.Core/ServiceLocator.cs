using System;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core
{
    /// <summary>
    /// Ref https://dotnetcoretutorials.com/2018/05/06/servicelocator-shim-for-net-core/
    /// </summary>
    public class ServiceLocator
    {
        private readonly IServiceProvider _currentServiceProvider;
        private static IServiceProvider _serviceProvider;

        public ServiceLocator(IServiceProvider currentServiceProvider)
        {
            _currentServiceProvider = currentServiceProvider;
        }

        public static ServiceLocator Current => new(_serviceProvider);

        public static void SetLocatorProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetInstance(Type serviceType)
        {
            return _currentServiceProvider.GetService(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return _currentServiceProvider.GetService<TService>();
        }
    }
}