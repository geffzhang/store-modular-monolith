using System;

namespace Common.Domain.Types
{
    public abstract class DomainNotificationEventBase<T> : IDomainNotificationEventBase<T>
        where T : IDomainEvent
    {
        public DomainNotificationEventBase(T domainEvent, Guid id)
        {
            Id = id;
            DomainEvent = domainEvent;
        }

        public T DomainEvent { get; }

        public Guid Id { get; }
    }
}