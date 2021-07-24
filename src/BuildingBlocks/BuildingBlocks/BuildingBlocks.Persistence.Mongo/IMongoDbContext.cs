using BuildingBlocks.Core.Messaging.Outbox;
using MongoDB.Driver;

namespace BuildingBlocks.Persistence.Mongo
{
    public interface IMongoDbContext
    {
        IMongoCollection<OutboxMessage> OutboxMessages { get; }
        MongoTransaction Transaction { get; set; }
    }
}