using System;

namespace BuildingBlocks.Core.Messaging
{
    public interface IMessage
    {
        Guid Id { get; set; }
        Guid CorrelationId { get; set; }
        DateTime OccurredOn { get; set; }
    }
}