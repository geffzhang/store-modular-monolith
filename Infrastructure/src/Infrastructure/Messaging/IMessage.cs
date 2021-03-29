using System;

namespace Infrastructure.Messaging
{
    public interface IMessage
    {
        Guid Id { get; set; }
        Guid CorrelationId { get; set; }
    }
}