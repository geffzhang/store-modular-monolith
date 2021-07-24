using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Persistence;
using MongoDB.Driver;

namespace BuildingBlocks.Persistence.Mongo
{
    public class MongoTransaction: ITransaction, IDisposable
    {
        public readonly IClientSessionHandle Session;

        public MongoTransaction(IClientSessionHandle session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await Session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await Session.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            Session?.Dispose();
        }
    }
}