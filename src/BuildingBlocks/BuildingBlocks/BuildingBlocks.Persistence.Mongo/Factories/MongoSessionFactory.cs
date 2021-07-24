using System.Threading.Tasks;
using MongoDB.Driver;

namespace BuildingBlocks.Persistence.Mongo.Factories
{
    internal sealed class MongoSessionFactory : IMongoSessionFactory
    {
        private readonly IMongoClient _client;

        public MongoSessionFactory(IMongoClient client)
        {
            _client = client;
        }

        public Task<IClientSessionHandle> CreateAsync()
        {
            return _client.StartSessionAsync();
        }
    }
}