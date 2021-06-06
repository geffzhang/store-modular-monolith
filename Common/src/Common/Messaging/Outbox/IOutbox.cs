using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Messaging.Outbox
{
    internal interface IOutbox
    {
        bool Enabled { get; }
        Task SaveAsync(IList<OutboxMessage> messages,CancellationToken cancellationToken = default);
        Task PublishUnsentAsync();
        Task CleanProcessedAsync(CancellationToken cancellationToken = default);
    }
}