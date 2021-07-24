namespace BuildingBlocks.Core.Messaging
{
    public interface IActionRejected : IMessage
    {
        string Code { get; }
        string Reason { get; }
    }
}