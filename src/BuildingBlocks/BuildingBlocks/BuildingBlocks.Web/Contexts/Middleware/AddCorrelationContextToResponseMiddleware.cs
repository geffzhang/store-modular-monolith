using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BuildingBlocks.Web.Contexts.Middleware
{
    public class AddCorrelationContextToResponseMiddleware : IMiddleware
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;
        private readonly CorrelationContextOptions _options;

        public AddCorrelationContextToResponseMiddleware(ICorrelationContextAccessor correlationContextAccessor,
            IOptions<CorrelationContextOptions> options)
        {
            _correlationContextAccessor = correlationContextAccessor;
            _options = options.Value;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            AddCorrelationHeaders(context.Response);

            return next(context);
        }

        private void AddCorrelationHeaders(HttpResponse response)
        {
            foreach (var (key, value) in _correlationContextAccessor.GetCorrelationContextValues())
            {
                response.Headers.Add(key, value.ToString());
            }

            response.Headers.TryAdd(_options.CorrelationContextHeaderKey,
                JsonConvert.SerializeObject(_correlationContextAccessor.CorrelationContext));
        }
    }
}