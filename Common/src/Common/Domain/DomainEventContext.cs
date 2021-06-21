using System.Collections.Generic;
using Common.Persistence.MSSQL;

namespace Common.Domain
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