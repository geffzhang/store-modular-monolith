using System;

namespace Common.Domain.Types
{
    public abstract class DomainNotificationBase<T> : IDomainNotificationEvent<T>
        where T : IDomainEvent
    {
        public DomainNotificationBase(T domainEvent, Guid id)
        {
            Id = id;
            DomainEvent = domainEvent;
        }

        public T DomainEvent { get; }

        public Guid Id { get; }
    }
}