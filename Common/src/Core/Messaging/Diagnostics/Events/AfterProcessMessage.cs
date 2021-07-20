namespace Common.Core.Messaging.Diagnostics.Events
{
    public class AfterProcessMessage
    {
        public AfterProcessMessage(IMessage eventData)
            => EventData = eventData;
        public IMessage EventData { get; }

    }
}