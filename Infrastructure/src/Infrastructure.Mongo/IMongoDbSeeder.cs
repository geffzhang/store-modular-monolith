using System.Threading.Tasks;
using MongoDB.Driver;

namespace Infrastructure.Mongo
{
    public interface IMongoDbSeeder
    {
        Task SeedAsync(IMongoDatabase database);
    }
}