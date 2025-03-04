using System;
using BuildingBlocks.Core.Domain.DomainEvents;

namespace BuildingBlocks.Core.Domain.DomainEventNotifications
{
    public abstract class DomainNotificationEventBase<TEventType> : IDomainEventNotification<TEventType> where TEventType : IDomainEvent
    {
        protected DomainNotificationEventBase(TEventType domainEvent, Guid id, Guid correlationId)
        {
            CorrelationId = correlationId;
            Id = id;
            DomainEvent = domainEvent;
        }

        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public TEventType DomainEvent { get; set; }
    }
}