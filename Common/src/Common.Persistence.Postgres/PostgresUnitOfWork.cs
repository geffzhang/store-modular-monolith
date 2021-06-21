using System;
using System.Threading.Tasks;
using Common.Domain.Dispatching;
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


        public async Task CommitTransactionAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public Task BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task RollbackTransaction()
        {
            throw new NotImplementedException();
        }
    }
}