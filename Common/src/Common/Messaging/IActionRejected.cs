namespace Common.Messaging.Events
{
    public interface IActionRejected : IEvent
    {
        string Code { get; }
        string Reason { get; }
    }
}