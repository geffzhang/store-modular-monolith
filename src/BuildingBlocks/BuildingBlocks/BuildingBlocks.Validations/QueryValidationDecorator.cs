using System.Threading.Tasks;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Messaging.Queries;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BuildingBlocks.Validations
{
    [Decorator]
    public class QueryValidationDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;
        private readonly ILogger<IQueryHandler<TQuery, TResult>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<TQuery> _validator;

        public QueryValidationDecorator(IQueryHandler<TQuery, TResult> handler,
            ILogger<IQueryHandler<TQuery, TResult>> logger,
            IHttpContextAccessor httpContextAccessor, IValidator<TQuery> validator)
        {
            _handler = handler;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _validator = validator;
        }

        public async Task<TResult> HandleAsync(TQuery query)
        {
            _logger.LogInformation(
                "[{Prefix}] Handle request={X-RequestData} and response={X-ResponseData}",
                nameof(QueryValidationDecorator<TQuery, TResult>), typeof(TQuery).Name, typeof(TResult).Name);

            _logger.LogDebug($"Handling {typeof(TQuery).FullName} with content {JsonConvert.SerializeObject(query)}");

            await _validator.ValidateAsync(query);

            var response = await _handler.HandleAsync(query);

            _logger.LogInformation($"Handled {typeof(TQuery).FullName}");

            return response;
        }
    }
}