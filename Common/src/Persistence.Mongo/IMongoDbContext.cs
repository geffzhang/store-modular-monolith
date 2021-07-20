using Common.Core.Messaging.Outbox;
using Common.Messaging.Outbox;
using MongoDB.Driver;

namespace Common.Persistence.Mongo
{
    public interface IMongoDbContext
    {
        IMongoCollection<OutboxMessage> OutboxMessages { get; }
        MongoTransaction Transaction { get; set; }
    }
}