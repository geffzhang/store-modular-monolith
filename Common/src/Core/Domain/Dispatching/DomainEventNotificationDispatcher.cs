using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Core.Domain.Dispatching
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
            List<Task> tasks = new List<Task>();
            foreach (var @event in events)
            {
                var handlerType = typeof(IDomainEventNotificationHandler<>).MakeGenericType(@event.GetType());
                var handlers = _serviceProvider.GetServices(handlerType);

                 tasks = handlers.Select(x => (Task) handlerType
                    .GetMethod(nameof(IDomainEventNotificationHandler<IDomainEventNotification>.HandleAsync))
                    ?.Invoke(x, new[] {@event})).ToList();
            }
            await Task.WhenAll(tasks);
        }
    }
}