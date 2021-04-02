using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.EFCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly IEnumerable<DbContext> _dbContexts;

        public UnitOfWork(IEnumerable<DbContext> dbContexts, ICommandProcessor commandProcessor)
        {
            _dbContexts = dbContexts ?? throw CoreException.NullArgument(nameof(dbContexts));
            _commandProcessor = commandProcessor ?? throw CoreException.NullArgument(nameof(commandProcessor));
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default,
            Guid? internalCommandId = null)
        {
            var count = 0;
            foreach (var dbContext in _dbContexts) count += await dbContext.SaveChangesAsync(cancellationToken);
            return count;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (var dbContext in _dbContexts) dbContext?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}