using System;
using BuildingBlocks.Core.Messaging.Outbox;
using MongoDB.Driver;

namespace BuildingBlocks.Persistence.Mongo
{
    public class MongoDbContext : IMongoDbContext
    {
        static MongoDbContext()
        {
            ConfigureMappings();
        }
        
        public MongoDbContext(IMongoDatabase db)
        {
            if (db == null) 
                throw new ArgumentNullException(nameof(db));
            
            OutboxMessages = db.GetCollection<OutboxMessage>("outbox");
            BuildOutboxIndexes();
        }

        private void BuildOutboxIndexes()
        {
            var indexBuilder = Builders<OutboxMessage>.IndexKeys;
            var indexKeys = indexBuilder.Ascending(e => e.Id);
            
            var index = new CreateIndexModel<OutboxMessage>(indexKeys, new CreateIndexOptions()
            {
                Unique = false,
                Name = "ix_id"
            });
            OutboxMessages.Indexes.CreateOne(index);
        }

        private static void ConfigureMappings()
        {
        }
        public IMongoCollection<OutboxMessage> OutboxMessages { get; }
        public MongoTransaction Transaction { get; set; }
    }
}