using System;
using Common.Messaging.Events;

namespace Common.Domain
{
    public interface IDomainNotificationEvent<out TEventType> : IDomainNotificationEvent
    {
        TEventType DomainEvent { get; }
    }

    public interface IDomainNotificationEvent : IEvent
    {
    }
    
}