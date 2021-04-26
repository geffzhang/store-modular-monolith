using System;

namespace Common.Messaging
{
    public interface IMessage
    {
        Guid Id { get; set; }
        Guid CorrelationId { get; set; }
    }

}