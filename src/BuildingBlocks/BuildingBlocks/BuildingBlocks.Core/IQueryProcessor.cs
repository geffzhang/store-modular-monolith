using System.Threading.Tasks;
using BuildingBlocks.Cqrs.Queries;

namespace BuildingBlocks.Core
{
    public interface IQueryProcessor
    {
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
        Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>;
    }
}