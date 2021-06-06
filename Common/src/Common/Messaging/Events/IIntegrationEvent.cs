using System;
using Common.Domain;

namespace Common.Messaging.Events
{
    public interface IIntegrationEvent : IEvent
    {
        DateTime OccurredOn { get; }
        Guid Id { get; set;}
        Guid CorrelationId { get; set; }
    }
}