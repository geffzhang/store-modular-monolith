using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Web.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Messaging.Events
{
    internal sealed class IntegrationEventDispatcher : IIntegrationEventDispatcher
    {
        private readonly IServiceScopeFactory _serviceFactory;
        private readonly ILogger<IntegrationEventDispatcher> _logger;

        public IntegrationEventDispatcher(IServiceScopeFactory serviceFactory, ILogger<IntegrationEventDispatcher> logger)
        {
            _serviceFactory = serviceFactory;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T @event) where T : class, IIntegrationEvent
        {
            if (@event is null)
            {
                return;
            }

            using var scope = _serviceFactory.CreateScope();
            if (@event.CorrelationId == Guid.Empty)
            {
                var context = scope.ServiceProvider.GetRequiredService<ICorrelationContextAccessor>();
                @event.CorrelationId = Guid.Parse(context.CorrelationContext.CorrelationId); 
            }

            if (typeof(T) == typeof(IIntegrationEvent))
            {
                var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(@event.GetType());
                var eventHandlers = scope.ServiceProvider.GetServices(handlerType);
                var handlerTasks = eventHandlers.Select(x => (Task) handlerType
                    .GetMethod(nameof(IIntegrationEventHandler<IIntegrationEvent>.HandleAsync))
                    ?.Invoke(x, new[] {@event}));
                
                await Task.WhenAll(handlerTasks);
                
                return;
            }

            var handlers = scope.ServiceProvider.GetServices<IIntegrationEventHandler<T>>();
            var tasks = handlers.Select(x => x.HandleAsync(@event));
            await Task.WhenAll(tasks);
        }
    }
}