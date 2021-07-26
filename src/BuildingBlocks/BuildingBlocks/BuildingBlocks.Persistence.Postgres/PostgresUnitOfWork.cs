using System;
using System.Threading.Tasks;
using BuildingBlocks.Core.Domain.DomainEvents;
using BuildingBlocks.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.Postgres
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