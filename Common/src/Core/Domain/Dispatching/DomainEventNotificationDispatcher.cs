using System;
using System.Threading.Tasks;
using Common.Core.Utils.Reflection;
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

        public Task DispatchAsync<T>(params T[] events) where T : class, IDomainEventNotification
        {
            foreach (var @event in events)
            {
                var handlerType = typeof(IDomainEventNotificationHandler<>).MakeGenericType(@event.GetType());

                var handlers = _serviceProvider.GetServices(handlerType);
                foreach (var handler in handlers)
                {
                    handlerType.GetMethod(
                            nameof(IDomainEventNotificationHandler<IDomainEventNotification>.HandleAsync))
                        ?.InvokeAsync(handler, new object[] {@event});
                }
            }

            return Task.CompletedTask;
        }
    }
}