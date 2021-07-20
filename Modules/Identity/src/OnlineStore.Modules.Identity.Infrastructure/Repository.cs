using System;
using Common.Core.Domain.Types;
using Common.Persistence.MSSQL;

namespace OnlineStore.Modules.Identity.Infrastructure
{
    public class Repository<TEntity, TId, TIdentity> : RepositoryBase<IdentityDbContext, TEntity, TId, TIdentity>
        where TEntity : AggregateRoot<TId, TIdentity>
        where TIdentity : IdentityBase<TId>
    {
        public Repository(IdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
    public class Repository<TEntity> : Repository<TEntity, Guid, AggregateId> where TEntity : AggregateRoot
    {
        public Repository(IdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
}