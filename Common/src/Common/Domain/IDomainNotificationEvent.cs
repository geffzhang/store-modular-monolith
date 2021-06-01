using System;
using Common.Messaging.Events;

namespace Common.Domain
{
    public interface IDomainNotificationEvent<TEventType> : IDomainNotificationEvent
    {
        TEventType DomainEvent { get; set; }
    }

    public interface IDomainNotificationEvent
    {
        Guid Id { get; set; }
    }
    
}