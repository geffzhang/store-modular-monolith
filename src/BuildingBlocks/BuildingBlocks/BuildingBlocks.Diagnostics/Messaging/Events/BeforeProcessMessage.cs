using BuildingBlocks.Core.Messaging;

namespace BuildingBlocks.Diagnostics.Messaging.Events
{
    public class BeforeProcessMessage
    {
        public BeforeProcessMessage(IMessage eventData)
            => EventData = eventData;
        public IMessage EventData { get; }
    }
}