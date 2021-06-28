using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Messaging.Outbox;
using Common.Serialization;
using Common.Utils.Extensions;
using Common.Utils.Reflection;
using Common.Web.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Common.Domain.Dispatching
{
    internal sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IDomainEventContext _domainEventContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public DomainEventDispatcher(IDomainEventContext domainEventContext, IServiceProvider serviceProvider,
            ICorrelationContextAccessor correlationContextAccessor)
        {
            _domainEventContext = domainEventContext;
            _serviceProvider = serviceProvider;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public async Task DispatchAsync(params IDomainEvent[] domainEvents)
        {
            IEnumerable<IDomainEvent> events = domainEvents is null || domainEvents.Any() == false
                ? _domainEventContext.GetDomainEvents()?.ToList()
                : domainEvents.ToList();

            if (events is null || events.Any() == false)
                return;

            foreach (var @event in events)
            {
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());
                var handlers = _serviceProvider.GetServices(handlerType);

                var tasks = handlers.Select(x => (Task) handlerType
                    .GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))
                    ?.Invoke(x, new[] {@event}));

                await Task.WhenAll(tasks);
            }

            var domainEventNotifications = new List<dynamic>();
            foreach (var domainEvent in events)
            {
                //https://jeremylindsayni.wordpress.com/2019/02/11/instantiating-a-c-object-from-a-string-using-activator-createinstance-in-net/
                Type domainEvenNotificationType = typeof(IDomainEventNotification<>);
                var domainNotificationWithGenericType = domainEvenNotificationType.MakeGenericType(domainEvent.GetType());

                var types = ReflectionHelper.GetAllTypesImplementingInterface(domainNotificationWithGenericType);
                foreach (var type in types)
                {

                    var domainEventNotification =
                        ReflectionHelper.CreateInstanceFromType<dynamic>(type, domainEvent, domainEvent.Id,
                           Guid.Parse(_correlationContextAccessor.CorrelationContext.CorrelationId));
                    if (domainEventNotification is null)
                        continue;

                    
                    domainEventNotifications.Add(domainEventNotification);
                }
            }

            //http://www.kamilgrzybek.com/design/the-outbox-pattern/
            //http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/

            //saving notification event to outbox in transaction and executing them as separate processes after commit 
            foreach (var domainEventNotification in domainEventNotifications)
            {
                var data = JsonConvert.SerializeObject(domainEventNotification,
                    new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});

                var outbox = _serviceProvider.GetRequiredService<IOutbox>();
                var outboxOptions = _serviceProvider.GetRequiredService<IOptions<OutboxOptions>>();
                var domainNotificationsMapper = _serviceProvider.GetRequiredService<IDomainNotificationsMapper>();
                var name = domainNotificationsMapper.GetName(domainEventNotification.GetType());

                if (outboxOptions.Value?.Enabled == false)
                    return;

                var outboxMessage = new OutboxMessage()
                {
                    Id = domainEventNotification.Id,
                    OccurredOn = domainEventNotification.DomainEvent.OccurredOn,
                    Type = domainEventNotification.GetType().AssemblyQualifiedName,
                    Name = name ?? domainEventNotification.GetType().Name.Underscore(),
                    Payload = data,
                    CorrelationId = domainEventNotification.CorrelationId,
                    ModuleName = ((object) domainEventNotification).GetModuleName()
                };
                await outbox.SaveAsync(new List<OutboxMessage> {outboxMessage});
            }
        }
    }
}