using System.Threading.Tasks;
using BuildingBlocks.Core.Messaging.Queries;

namespace BuildingBlocks.Core.Messaging
{
    public interface IQueryProcessor
    {
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
        Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>;
    }
}