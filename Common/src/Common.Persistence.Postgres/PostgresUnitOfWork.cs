using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.Postgres
{
    public abstract class PostgresUnitOfWork<T> : IUnitOfWork where T : DbContext
    {
        private readonly DbContext _dbContext;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public PostgresUnitOfWork(DbContext dbContext, IDomainEventDispatcher domainEventDispatcher)
        {
            _dbContext = dbContext;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default,
            Guid? internalCommandId = null)
        {
            await _domainEventDispatcher.DispatchAsync();
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}