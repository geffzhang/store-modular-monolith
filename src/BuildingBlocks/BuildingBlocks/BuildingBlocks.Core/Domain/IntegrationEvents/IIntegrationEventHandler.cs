using BuildingBlocks.Core.Messaging;

namespace BuildingBlocks.Core.Domain.IntegrationEvents
{
    public interface IIntegrationEventHandler<in T> : IMessageHandler<T> where T : IMessage
    {
    }
}