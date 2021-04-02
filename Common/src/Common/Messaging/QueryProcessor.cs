using System.Threading.Tasks;
using Common.Messaging.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging
{
    internal sealed class QueryProcessor : IQueryProcessor
    {
        private readonly IServiceScopeFactory _serviceFactory;

        public QueryProcessor(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            using var scope = _serviceFactory.CreateScope();
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            var handler = scope.ServiceProvider.GetRequiredService(handlerType);
            if (handler is null) return default;

            return await (Task<TResult>) handlerType
                .GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync))?
                .Invoke(handler, new[] {query});

            // dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);
            // return await handler.HandleAsync((dynamic) query);
        }

        public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>
        {
            using var scope = _serviceFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return await handler.HandleAsync(query);
        }
    }
}