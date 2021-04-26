using System.Threading.Tasks;
using MongoDB.Driver;

namespace Common.Mongo
{
    public interface IMongoDbSeeder
    {
        Task SeedAsync(IMongoDatabase database);
    }
}