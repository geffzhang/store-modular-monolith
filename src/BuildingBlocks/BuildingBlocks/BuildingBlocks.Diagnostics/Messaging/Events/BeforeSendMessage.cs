using BuildingBlocks.Core.Messaging;

namespace BuildingBlocks.Diagnostics.Messaging.Events
{
    public class BeforeSendMessage
    {
        public BeforeSendMessage(IMessage eventData)
            => EventData = eventData;
        public IMessage EventData { get; }
    }
}