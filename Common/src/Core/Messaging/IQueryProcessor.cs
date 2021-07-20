using System.Threading.Tasks;
using Common.Core.Messaging.Queries;

namespace Common.Core.Messaging
{
    public interface IQueryProcessor
    {
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
        Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>;
    }
}