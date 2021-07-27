using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Cqrs.Commands;
using BuildingBlocks.Cqrs.Events;
using BuildingBlocks.Cqrs.Queries;
using Google.Protobuf;

namespace BuildingBlocks.Cqrs
{
    public class RequestProcessor<TRequest, TResponse> : IRequestProcessor<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _requestHandlers;
        private readonly IEnumerable<IRequestMiddleware<TRequest, TResponse>> _middlewares;

        public RequestProcessor(IServiceProvider serviceProvider)
        {
            _requestHandlers = (IEnumerable<IRequestHandler<TRequest, TResponse>>) serviceProvider.GetService(typeof(IEnumerable<IRequestHandler<TRequest, TResponse>>));

            _middlewares = (IEnumerable<IRequestMiddleware<TRequest, TResponse>>)
                serviceProvider.GetService(typeof(IEnumerable<IRequestMiddleware<TRequest, TResponse>>));
        }

        public Task<TResponse> ProcessAsync(TRequest message, CancellationToken cancellationToken)
        {
            return RunMiddleware(message, HandleMessage, cancellationToken);
        }

        private async Task<TResponse> HandleMessage(TRequest request, CancellationToken cancellationToken)
        {
            var type = typeof(TRequest);

            if (!_requestHandlers.Any())
            {
                throw new ArgumentException(
                    $"No handler of signature {typeof(IRequestHandler<,>).Name} was found for {typeof(TRequest).Name}",
                    typeof(TRequest).FullName);
            }

            if (typeof(IEvent).IsAssignableFrom(type) || typeof(IEvent<>).IsAssignableFrom(type))
            {
                var tasks = _requestHandlers.Select(r => r.HandleAsync(request, cancellationToken));
                var result = default(TResponse);

                foreach (var task in tasks)
                {
                    result = await task;
                }

                return result;
            }

            if (typeof(IQuery<TResponse>).IsAssignableFrom(type) || typeof(ICommand<>).IsAssignableFrom(type) ||
                typeof(ICommand).IsAssignableFrom(type))
            {
                return await _requestHandlers.Single().HandleAsync(request, cancellationToken);
            }

            throw new ArgumentException(
                $"{typeof(TRequest).Name} is not a known type of {typeof(IMessage<>).Name} - Query, Command or Event",
                typeof(TRequest).FullName);
        }

        private Task<TResponse> RunMiddleware(TRequest request, HandleRequestDelegate<TRequest, TResponse>
            handleRequestHandlerCall, CancellationToken cancellationToken)
        {
            HandleRequestDelegate<TRequest, TResponse> next = null;

            next = _middlewares.Reverse().Aggregate(handleRequestHandlerCall, (requestDelegate, middleware) =>
                ((req, ct) => middleware.RunAsync(req, ct, requestDelegate)));

            return next.Invoke(request, cancellationToken);
        }
    }
}