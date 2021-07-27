using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Cqrs.Queries
{
    public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
        where TQuery : IRequest<TResponse>
    {
        public Task<TResponse> HandleAsync(TQuery message, CancellationToken cancellationToken = default)
        {
            return HandleQueryAsync(message, cancellationToken);
        }

        protected abstract Task<TResponse>
            HandleQueryAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}