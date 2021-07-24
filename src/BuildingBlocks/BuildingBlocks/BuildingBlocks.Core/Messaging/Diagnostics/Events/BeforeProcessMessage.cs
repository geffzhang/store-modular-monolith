namespace BuildingBlocks.Core.Messaging.Diagnostics.Events
{
    public class BeforeProcessMessage
    {
        public BeforeProcessMessage(IMessage eventData)
            => EventData = eventData;
        public IMessage EventData { get; }
    }
}