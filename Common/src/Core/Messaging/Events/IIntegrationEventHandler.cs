namespace Common.Core.Messaging.Events
{
    public interface IIntegrationEventHandler<in T> : IMessageHandler<T> where T : IMessage
    {
    }
}