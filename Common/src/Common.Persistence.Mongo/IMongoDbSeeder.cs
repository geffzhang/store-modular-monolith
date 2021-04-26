using System.Threading.Tasks;
using MongoDB.Driver;

namespace Common.Persistence.Mongo
{
    public interface IMongoDbSeeder
    {
        Task SeedAsync(IMongoDatabase database);
    }
}