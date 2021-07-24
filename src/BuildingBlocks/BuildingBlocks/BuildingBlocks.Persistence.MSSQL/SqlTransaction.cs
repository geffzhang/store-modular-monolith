using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.Persistence.MSSQL
{
    public class SqlTransaction : ITransaction, IDisposable
    {
        private IDbContextTransaction _transaction;

        public SqlTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        public Task CommitAsync(CancellationToken cancellationToken = default) =>
            _transaction.CommitAsync(cancellationToken);

        public Task RollbackAsync(CancellationToken cancellationToken = default) =>
            _transaction.RollbackAsync(cancellationToken);

        public void Dispose()
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }
}