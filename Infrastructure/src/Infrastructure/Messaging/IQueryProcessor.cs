using System.Threading.Tasks;
using Infrastructure.Messaging.Queries;

namespace Infrastructure.Messaging
{
    public interface IQueryProcessor
    {
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
        Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>;
    }
}