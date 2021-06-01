using System;

namespace Common.Messaging.Events
{
    public abstract class IntegrationEventBase : IIntegrationEvent
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.Now;
    }
}