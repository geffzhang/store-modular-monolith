using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Messaging.Events;
using Common.Messaging.Outbox;
using Common.Serialization;
using Common.Utils.Extensions;
using Common.Web.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Common.Domain.Dispatching
{
    internal sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task DispatchAsync(params IDomainEvent[] domainEvents)
        {
            if (domainEvents is null || !domainEvents.Any())
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();

            var domainEventNotifications = new List<IDomainEventNotification<IDomainEvent>>();
            foreach (var domainEvent in domainEvents)
            {
                //https://jeremylindsayni.wordpress.com/2019/02/11/instantiating-a-c-object-from-a-string-using-activator-createinstance-in-net/
                Type domainEvenNotificationType = typeof(IDomainEventNotification<>);
                var domainNotificationWithGenericType =
                    domainEvenNotificationType.MakeGenericType(domainEvent.GetType());
                var domainNotification =
                    Activator.CreateInstance(domainNotificationWithGenericType) as
                        IDomainEventNotification<IDomainEvent>;

                if (domainNotification is null)
                    continue;

                domainNotification.DomainEvent = domainEvent;
                domainNotification.Id = domainEvent.Id;

                domainEventNotifications.Add(domainNotification);
            }

            foreach (var @event in domainEvents)
            {
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());
                var handlers = scope.ServiceProvider.GetServices(handlerType);

                var tasks = handlers.Select(x => (Task) handlerType
                    .GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))
                    ?.Invoke(x, new[] {@event}));

                await Task.WhenAll(tasks);
            }

            //http://www.kamilgrzybek.com/design/the-outbox-pattern/
            //http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/

            //saving notification event to outbox in transaction and executing them as separate processes after commit 
            foreach (var domainEventNotification in domainEventNotifications)
            {
                var data = JsonConvert.SerializeObject(domainEventNotification, new JsonSerializerSettings
                {
                    ContractResolver = new AllPropertiesContractResolver()
                });

                var outbox = _serviceProvider.GetRequiredService<IOutbox>();
                var outboxOptions = _serviceProvider.GetRequiredService<OutboxOptions>();
                var context = _serviceProvider.GetRequiredService<IContext>();
                var domainNotificationsMapper = _serviceProvider.GetRequiredService<IDomainNotificationsMapper>();
                var name = domainNotificationsMapper.GetName(domainEventNotification.GetType());

                if (outboxOptions.Enabled == false)
                    return;

                var outboxMessage = new OutboxMessage()
                {
                    Id = domainEventNotification.Id,
                    CorrelationId = context.CorrelationId,
                    OccurredOn = domainEventNotification.DomainEvent.OccurredOn,
                    Type = domainEventNotification.GetType().AssemblyQualifiedName,
                    Name = name ?? domainEventNotification.GetType().Name.Underscore(),
                    Payload = data
                };

                await outbox.SaveAsync(new List<OutboxMessage> {outboxMessage});
            }
        }
    }
}