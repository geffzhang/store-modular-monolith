using System.Collections.Generic;
using BuildingBlocks.Persistence.MSSQL;

namespace BuildingBlocks.Core.Domain.DomainEvents
{
    public class DomainEventContext : IDomainEventContext
    {
        private readonly ISqlDbContext _dbContext;

        public DomainEventContext(ISqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<IDomainEvent> GetDomainEvents()
        {
            return _dbContext.GetDomainEvents();
        }
    }
}