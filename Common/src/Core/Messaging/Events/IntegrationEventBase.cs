using System;

namespace Common.Core.Messaging.Events
{
    public abstract class IntegrationEventBase : IIntegrationEvent
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.Now;
    }
}