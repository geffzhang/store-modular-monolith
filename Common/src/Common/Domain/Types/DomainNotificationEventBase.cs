using System;

namespace Common.Domain.Types
{
    public abstract class DomainNotificationEventBase<TEventType> : IDomainEventNotification<TEventType>
        where TEventType : IDomainEvent
    {
        protected DomainNotificationEventBase()
        {
        }

        protected DomainNotificationEventBase(TEventType domainEvent, Guid id)
        {
            Id = id;
            DomainEvent = domainEvent;
        }

        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public TEventType DomainEvent { get; set; }
    }
}