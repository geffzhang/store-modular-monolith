using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Domain
{
    public class DomainNotificationEventDispatcher: IDomainNotificationEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainNotificationEventDispatcher(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task DispatchAsync(params IDomainNotificationEvent[] domainNotificationEvent)
        {
            using var scope = _serviceProvider.CreateScope();

            foreach (var @event in domainNotificationEvent)
            {
                var handlerType = typeof(IDomainNotificationEventHandler<>).MakeGenericType(@event.GetType());
                var handlers = scope.ServiceProvider.GetServices(handlerType);

                var tasks = handlers.Select(x => (Task) handlerType
                    .GetMethod(nameof(IDomainNotificationEventHandler<IDomainNotificationEvent>.HandleAsync))
                    ?.Invoke(x, new[] {@event}));

                await Task.WhenAll(tasks);
            }
        }
    }
}