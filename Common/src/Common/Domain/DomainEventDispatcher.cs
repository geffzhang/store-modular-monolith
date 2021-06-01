using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Scheduling;
using Common.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Common.Domain
{
    internal sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task DispatchAsync(params IDomainEvent[] events)
        {
            if (events is null || !events.Any())
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();

            var domainEventNotifications = new List<IDomainNotificationEvent<IDomainEvent>>();
            foreach (var domainEvent in events)
            {
                //https://jeremylindsayni.wordpress.com/2019/02/11/instantiating-a-c-object-from-a-string-using-activator-createinstance-in-net/
                Type domainEvenNotificationType = typeof(IDomainNotificationEvent<>);
                var domainNotificationWithGenericType =
                    domainEvenNotificationType.MakeGenericType(domainEvent.GetType());
                var domainNotification =
                    Activator.CreateInstance(domainNotificationWithGenericType) as
                        IDomainNotificationEvent<IDomainEvent>;

                if (domainNotification is null)
                    continue;

                domainNotification.DomainEvent = domainEvent;
                domainNotification.Id = domainEvent.Id;

                domainEventNotifications.Add(domainNotification);
            }

            foreach (var @event in events)
            {
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());
                var handlers = scope.ServiceProvider.GetServices(handlerType);

                var tasks = handlers.Select(x => (Task) handlerType
                    .GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))
                    ?.Invoke(x, new[] {@event}));

                await Task.WhenAll(tasks);
            }

            foreach (var domainEventNotification in domainEventNotifications)
            {
                var data = JsonConvert.SerializeObject(domainEventNotification, new JsonSerializerSettings
                {
                    ContractResolver = new AllPropertiesContractResolver()
                });

                var scheduler = _serviceProvider.GetRequiredService<IMessagesScheduler>();
                var message = new MessageSerializedObject(domainEventNotification.GetType().AssemblyQualifiedName,
                    data, "");
                scheduler?.Schedule(message, TimeSpan.Zero);
            }
        }
    }
}