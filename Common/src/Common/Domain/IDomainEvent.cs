using System;
using Common.Messaging.Events;

namespace Common.Domain
{
    public interface IDomainEvent : IEvent
    {
        DateTime CreatedAt { get; }
    }
}