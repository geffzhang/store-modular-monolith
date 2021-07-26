using BuildingBlocks.Core.Messaging;

namespace BuildingBlocks.Diagnostics.Messaging.Events
{
    public class AfterProcessMessage
    {
        public AfterProcessMessage(IMessage eventData)
            => EventData = eventData;
        public IMessage EventData { get; }

    }
}