using Common.Messaging.Events;

namespace Common.Messaging
{
    public interface IActionRejected : IIntegrationEvent
    {
        string Code { get; }
        string Reason { get; }
    }
}