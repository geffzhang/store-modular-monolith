using System;
using Common.Persistence.Mongo;
using Common.Tests.Integration.Helpers;
using MongoDB.Driver;

namespace Common.Tests.Integration.Fixtures
{
    public class MongoFixture : IDisposable
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

        public void Dispose()
        {
            _client.DropDatabase(_databaseName);
        }
    }
}