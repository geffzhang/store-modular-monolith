using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Messaging.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Domain.Dispatching
{
    public class DomainEventNotificationDispatcher : IDomainEventNotificationDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventNotificationDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(params IDomainEventNotification[] events)
        {
            foreach (var @event in events)
            {
                var handlerType = typeof(IDomainEventNotificationHandler<>).MakeGenericType(@event.GetType());
                var handlers = _serviceProvider.GetServices(handlerType);

                var tasks = handlers.Select(x => (Task) handlerType
                    .GetMethod(nameof(IDomainEventNotificationHandler<IDomainEventNotification>.HandleAsync))
                    ?.Invoke(x, new[] {@event}));

                await Task.WhenAll(tasks);
            }
        }
    }
}