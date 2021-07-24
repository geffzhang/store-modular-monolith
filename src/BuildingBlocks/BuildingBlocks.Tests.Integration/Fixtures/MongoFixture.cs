using System.Threading.Tasks;
using BuildingBlocks.Persistence.Mongo;
using BuildingBlocks.Tests.Integration.Helpers;
using MongoDB.Driver;
using Xunit;

namespace BuildingBlocks.Tests.Integration.Fixtures
{
    public class MongoFixture : IAsyncLifetime
    {
        private readonly MongoClient _client;
        private readonly string _databaseName;

        public MongoFixture()
        {
            var options = OptionsHelper.GetOptions<MongoOptions>("mongo");
            _client = new MongoClient(options.ConnectionString);
            _databaseName = options.Database;
        }

        public IMongoDatabase Database => _client.GetDatabase(_databaseName);

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _client.DropDatabase(_databaseName);
            return Task.CompletedTask;
        }
    }
}