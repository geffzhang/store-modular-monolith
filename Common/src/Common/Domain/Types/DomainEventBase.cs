using System;

namespace Common.Domain.Types
{
    public abstract class DomainEventBase<TId> : IDomainEvent
    {
        public DomainEventBase()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public TId Id { get; }

        public DateTime CreatedAt { get; }
    }

    public abstract class DomainEventBase : DomainEventBase<Guid>
    {
    }
}