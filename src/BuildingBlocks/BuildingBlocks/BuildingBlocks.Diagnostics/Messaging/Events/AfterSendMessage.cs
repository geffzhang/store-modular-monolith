using BuildingBlocks.Core.Messaging;

namespace BuildingBlocks.Diagnostics.Messaging.Events
{
    public class AfterSendMessage
    {
        public AfterSendMessage(IMessage eventData)
            => EventData = eventData;
        public IMessage EventData { get; }
    }
}