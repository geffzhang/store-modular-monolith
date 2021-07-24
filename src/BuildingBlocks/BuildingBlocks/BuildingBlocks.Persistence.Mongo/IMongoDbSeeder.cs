using System.Threading.Tasks;
using MongoDB.Driver;

namespace BuildingBlocks.Persistence.Mongo
{
    public interface IMongoDbSeeder
    {
        Task SeedAsync(IMongoDatabase database);
    }
}