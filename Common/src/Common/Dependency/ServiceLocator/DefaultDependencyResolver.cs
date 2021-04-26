using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Dependency
{
    public class DefaultDependencyResolver : IDependencyResolver
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DefaultDependencyResolver(IServiceProvider serviceProvider, ILoggerFactory loggerFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = loggerFactory.CreateLogger<DefaultDependencyResolver>();
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public DefaultDependencyResolver(IServiceProvider serviceProvider)
            : this(serviceProvider, serviceProvider.GetRequiredService<ILoggerFactory>(),
                serviceProvider.GetRequiredService<IHttpContextAccessor>())
        {
        }

        public object Resolve(Type type)
        {
            IServiceProvider currentServiceProvider;

            // When the call to resolve the given type is made within an HTTP Request, use the request scope service provider
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext != null)
            {
                _logger.LogDebug("The service {0} will be requested from the per-request scope", type);
                currentServiceProvider = httpContext.RequestServices;
            }
            else
            {
                // otherwise use the app wide scope provider
                _logger.LogDebug("The service {0} will be requested from the app scope", type);
                currentServiceProvider = _serviceProvider;
            }

            return currentServiceProvider.GetService(type);
        }

    }
}