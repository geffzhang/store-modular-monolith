using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain;
using Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.EFCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public UnitOfWork(DbContext dbContext, IDomainEventDispatcher domainEventDispatcher)
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