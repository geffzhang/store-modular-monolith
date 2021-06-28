using System;
using Common.Messaging.Events;

namespace Common.Domain
{
    public interface IDomainEventNotification<TEventType> : IDomainEventNotification
    {
        TEventType DomainEvent { get; set; }
    }

    public interface IDomainEventNotification 
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}