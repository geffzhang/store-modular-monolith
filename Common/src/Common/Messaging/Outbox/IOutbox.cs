using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Messaging.Outbox
{
    public interface IOutbox
    {
        bool Enabled { get; }
        Task SaveAsync(IList<OutboxMessage> messages, CancellationToken cancellationToken = default);
        Task PublishUnsentAsync();
        Task<IEnumerable<OutboxMessage>> GetAllOutboxMessages(string moduleName = default);
        Task CleanProcessedAsync(CancellationToken cancellationToken = default);
    }
}