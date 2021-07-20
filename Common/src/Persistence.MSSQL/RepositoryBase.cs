using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Core.Domain.Types;
using Common.Core.Exceptions;
using Common.Core.Persistence;
using Common.Core.Persistence.Specification;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.MSSQL
{
    public class RepositoryBase<TDbContext, TEntity, TId, TIdentity> : IRepository<TEntity, TId, TIdentity>
        where TEntity : AggregateRoot<TId, TIdentity>
        where TDbContext : DbContext, ISqlDbContext
        where TIdentity : IdentityBase<TId>
    {
        protected RepositoryBase(TDbContext dbContext)
        {
            DbContext = dbContext ?? throw CoreException.NullArgument(nameof(dbContext));
        }

        protected TDbContext DbContext { get; }

        public void Add(TEntity entity)
        {
            DbContext.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            DbContext.Set<TEntity>().AddRange(entities);
        }

        public virtual IAsyncEnumerable<TEntity> FindAllAsync(ISpecification<TEntity> specification = null)
        {
            var queryable = DbContext.Set<TEntity>().AsQueryable();
            if (specification == null)
            {
                return queryable
                    .AsNoTracking()
                    .AsAsyncEnumerable();
            }

            var queryableWithInclude = specification.Includes
                .Aggregate(queryable, (current, include) => current.Include(include));

            return queryableWithInclude
                .Where(specification.Criteria)
                .AsNoTracking()
                .AsAsyncEnumerable();
        }

        public void Remove(TEntity entity)
        {
            DbContext.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbContext.Set<TEntity>().RemoveRange(entities);
        }

        public void Update(TEntity entity)
        {
            DbContext.Set<TEntity>().Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            DbContext.Set<TEntity>().UpdateRange(entities);
        }

        public TEntity FindById(TIdentity id)
        {
            return DbContext.Set<TEntity>().SingleOrDefault(e => e.Id == id);
        }

        public async Task<TEntity> FindOneAsync(ISpecification<TEntity> spec)
        {
            var specificationResult = GetQuery(DbContext.Set<TEntity>(), spec);

            return await specificationResult.FirstOrDefaultAsync();
        }

        public async Task<List<TEntity>> FindAsync(ISpecification<TEntity> spec)
        {
            var specificationResult = GetQuery(DbContext.Set<TEntity>(), spec);

            return await specificationResult.ToListAsync();
        }

        public async ValueTask<long> CountAsync(ISpecification<TEntity> spec)
        {
            spec.IsPagingEnabled = false;
            var specificationResult = GetQuery(DbContext.Set<TEntity>(), spec);

            return await ValueTask.FromResult(specificationResult.LongCount());
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await DbContext.Set<TEntity>().AddAsync(entity);

            await DbContext.SaveChangesAsync();

            return entity;
        }

        public async Task RemoveAsync(TEntity entity)
        {
            DbContext.Set<TEntity>().Remove(entity);

            await DbContext.SaveChangesAsync();
        }

        private static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,
            ISpecification<TEntity> specification)
        {
            var query = inputQuery;

            if (specification.Criteria is not null)
            {
                query = query.Where(specification.Criteria);
            }

            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            if (specification.OrderBy is not null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending is not null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            if (specification.GroupBy is not null)
            {
                query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
            }

            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip - 1)
                    .Take(specification.Take);
            }

            return query;
        }
    }

    public class RepositoryBase<TDbContext, TEntity> : RepositoryBase<TDbContext, TEntity, Guid, AggregateId>
        where TEntity : AggregateRoot
        where TDbContext : DbContext, ISqlDbContext
    {
        protected RepositoryBase(TDbContext dbContext) : base(dbContext)
        {
        }
    }
}