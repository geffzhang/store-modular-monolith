using System.Threading.Tasks;
using MongoDB.Driver;

namespace Common.Persistence.Mongo
{
    public interface IMongoSessionFactory
    {
        Task<IClientSessionHandle> CreateAsync();
    }
}