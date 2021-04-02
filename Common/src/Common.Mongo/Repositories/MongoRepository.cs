using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Messaging.Queries;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Common.Mongo.Repositories
{
    internal class MongoRepository<TEntity, TIdentifiable> : IMongoRepository<TEntity, TIdentifiable>
        where TEntity : IIdentifiable<TIdentifiable>
    {
        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            Collection = database.GetCollection<TEntity>(collectionName);
        }

        public IMongoCollection<TEntity> Collection { get; }

        public Task<TEntity> GetAsync(TIdentifiable id)
        {
            return GetAsync(e => e.Id.Equals(id));
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Collection.Find(predicate).SingleOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Collection.Find(predicate).ToListAsync();
        }

        public Task<Paged<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate,
            TQuery query) where TQuery : IPagedQuery
        {
            return Collection.AsQueryable().Where(predicate).PaginateAsync(query);
        }

        public Task AddAsync(TEntity entity)
        {
            return Collection.InsertOneAsync(entity);
        }

        public Task UpdateAsync(TEntity entity)
        {
            return UpdateAsync(entity, e => e.Id.Equals(entity.Id));
        }

        public Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate)
        {
            return Collection.ReplaceOneAsync(predicate, entity);
        }

        public Task DeleteAsync(TIdentifiable id)
        {
            return DeleteAsync(e => e.Id.Equals(id));
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Collection.DeleteOneAsync(predicate);
        }

        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Collection.Find(predicate).AnyAsync();
        }
    }
}