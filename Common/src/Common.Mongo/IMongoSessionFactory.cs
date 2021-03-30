using System.Threading.Tasks;
using MongoDB.Driver;

namespace Common.Mongo
{
    public interface IMongoSessionFactory
    {
        Task<IClientSessionHandle> CreateAsync();
    }
}