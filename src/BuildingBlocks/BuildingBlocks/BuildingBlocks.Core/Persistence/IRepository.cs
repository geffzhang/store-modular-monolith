using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingBlocks.Core.Domain.Types;
using BuildingBlocks.Core.Persistence.Specification;

namespace BuildingBlocks.Core.Persistence
{
    public interface IRepository<TEntity, TId, in TIdentity>
        where TEntity : AggregateRoot<TId, TIdentity>
        where TIdentity : IdentityBase<TId>
    {
        IAsyncEnumerable<TEntity> FindAllAsync(ISpecification<TEntity> specification = null);
        Task<TEntity> FindOneAsync(ISpecification<TEntity> spec);
        Task<List<TEntity>> FindAsync(ISpecification<TEntity> spec);
        Task<TEntity> AddAsync(TEntity entity);
        Task RemoveAsync(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        TEntity FindById(TIdentity id);
    }
}