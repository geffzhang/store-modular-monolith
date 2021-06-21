using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Messaging.Events;
using Common.Messaging.Outbox;
using Common.Persistence.MSSQL;
using Common.Serialization;
using Common.Utils.Extensions;
using Common.Web.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Common.Domain.Dispatching
{
    internal sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IDomainEventContext _domainEventContext;
        private readonly IServiceScopeFactory _scopeFactory;

        public DomainEventDispatcher(IDomainEventContext domainEventContext, IServiceScopeFactory scopeFactory)
        {
            _domainEventContext = domainEventContext;
            _scopeFactory = scopeFactory;
        }

        public async Task DispatchAsync(params IDomainEvent[] domainEvents)
        {
            IEnumerable<IDomainEvent> events = domainEvents?.ToList();

            if (events is null || events.Any() == false)
            {
                events = _domainEventContext.GetDomainEvents();
            }

            var scope = _scopeFactory.CreateScope();
            
            var domainEventNotifications = new List<IDomainEventNotification<IDomainEvent>>();
            foreach (var domainEvent in events)
            {
                //https://jeremylindsayni.wordpress.com/2019/02/11/instantiating-a-c-object-from-a-string-using-activator-createinstance-in-net/
                Type domainEvenNotificationType = typeof(IDomainEventNotification<>);
                var domainNotificationWithGenericType =
                    domainEvenNotificationType.MakeGenericType(domainEvent.GetType());
                
                var domainNotification =
                    scope.ServiceProvider.GetService(domainNotificationWithGenericType) as IDomainEventNotification<IDomainEvent>;

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

            //http://www.kamilgrzybek.com/design/the-outbox-pattern/
            //http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/

            //saving notification event to outbox in transaction and executing them as separate processes after commit 
            foreach (var domainEventNotification in domainEventNotifications)
            {
                var data = JsonConvert.SerializeObject(domainEventNotification, new JsonSerializerSettings
                {
                    ContractResolver = new AllPropertiesContractResolver()
                });

                var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
                var outboxOptions = scope.ServiceProvider.GetRequiredService<OutboxOptions>();
                var context = scope.ServiceProvider.GetRequiredService<IContext>();
                var domainNotificationsMapper = scope.ServiceProvider.GetRequiredService<IDomainNotificationsMapper>();
                var name = domainNotificationsMapper.GetName(domainEventNotification.GetType());

                if (outboxOptions.Enabled == false)
                    return;

                var outboxMessage = new OutboxMessage()
                {
                    Id = domainEventNotification.Id,
                    OccurredOn = domainEventNotification.DomainEvent.OccurredOn,
                    Type = domainEventNotification.GetType().AssemblyQualifiedName,
                    Name = name ?? domainEventNotification.GetType().Name.Underscore(),
                    Payload = data,
                    ModuleName = domainEventNotification.GetModuleName()
                };
                await outbox.SaveAsync(new List<OutboxMessage> {outboxMessage});
            }
        }
    }
}