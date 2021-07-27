using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Messaging.Serialization;
using BuildingBlocks.Cqrs;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Validations
{
    public class CqrsRequestValidatorMiddleware<TRequest, TResponse> : IRequestMiddleware<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        private readonly ILogger<CqrsRequestValidatorMiddleware<TRequest, TResponse>> _logger;
        private readonly IMessageSerializer _serializer;
        private readonly IServiceProvider _serviceProvider;

        public CqrsRequestValidatorMiddleware(ILogger<CqrsRequestValidatorMiddleware<TRequest, TResponse>> logger,
            IMessageSerializer serializer, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serializer = serializer;
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> RunAsync(TRequest request, CancellationToken cancellationToken,
            HandleRequestDelegate<TRequest, TResponse> next)
        {
            var validator = _serviceProvider.GetService<IValidator<TRequest>>();
            if (validator is null)
                return await next(request, cancellationToken);

            _logger.LogInformation(
                "[{Prefix}] Handle request={X-RequestData} and response={X-ResponseData}",
                nameof(CqrsRequestValidatorMiddleware<TRequest, TResponse>), typeof(TRequest).Name,
                typeof(TResponse).Name);

            _logger.LogDebug($"Handling {typeof(TRequest).FullName} with content {_serializer.Serialize(request)}");

            await validator.HandleValidation(request);

            var response = await next(request, cancellationToken);

            _logger.LogInformation($"Handled {typeof(TRequest).FullName}");
            return response;
        }
    }
}