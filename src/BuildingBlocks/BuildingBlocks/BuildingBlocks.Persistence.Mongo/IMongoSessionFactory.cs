using System.Threading.Tasks;
using MongoDB.Driver;

namespace BuildingBlocks.Persistence.Mongo
{
    public interface IMongoSessionFactory
    {
        Task<IClientSessionHandle> CreateAsync();
    }
}