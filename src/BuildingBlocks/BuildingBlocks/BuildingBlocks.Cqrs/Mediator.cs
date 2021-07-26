using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Cqrs
{
    public class Mediator : IMediator
    {
        private readonly IServiceFactory _serviceFactory;

        public Mediator(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            var targetType = request.GetType();
            var targetHandler = typeof(IRequestProcessor<,>).MakeGenericType(targetType, typeof(TResponse));
            var instance = _serviceFactory.GetInstance(targetHandler);

            var result = InvokeInstanceAsync(instance, request, targetHandler, cancellationToken);

            return result;
        }

        private Task<TResponse> InvokeInstanceAsync<TResponse>(object instance, IRequest<TResponse> request,
            Type targetHandler, CancellationToken cancellationToken)
        {
            var method = instance.GetType()
                .GetTypeInfo()
                .GetMethod(nameof(IRequestProcessor<IRequest<TResponse>, TResponse>.HandleAsync));

            if (method == null)
            {
                throw new ArgumentException($"{instance.GetType().Name} is not a known {targetHandler.Name}",
                    instance.GetType().FullName);
            }

            return (Task<TResponse>) method.Invoke(instance, new object[] {request, cancellationToken});
        }
    }
}