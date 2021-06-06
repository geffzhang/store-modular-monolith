using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using Common.Domain.Dispatching;

namespace Common.Persistence.MSSQL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlDbContext _dbContext;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public UnitOfWork(ISqlDbContext dbContext, IDomainEventDispatcher domainEventDispatcher)
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