using System.Threading.Tasks;

namespace Common.Messaging.Outbox
{
    internal interface IOutbox
    {
        bool Enabled { get; }
        Task SaveAsync(params IMessage[] messages);
        Task PublishUnsentAsync();
    }
}