﻿using System;

namespace BuildingBlocks.Core.Domain.IntegrationEvents
{
    public abstract class IntegrationEventBase : IIntegrationEvent
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.Now;
    }
}