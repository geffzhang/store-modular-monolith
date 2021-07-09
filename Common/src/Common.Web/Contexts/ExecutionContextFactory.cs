using System;
using Common.Web.Extensions;
using Microsoft.AspNetCore.Http;

namespace Common.Web.Contexts
{
    public class ExecutionContextFactory : IExecutionContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpTraceId _traceId;

        public ExecutionContextFactory(IHttpContextAccessor httpContextAccessor, IHttpTraceId traceId)
        {
            _httpContextAccessor = httpContextAccessor;
            _traceId = traceId;
        }

        public ExecutionContext Create()
        {
            var context = new ExecutionContext
            {
                Name =
                    $"{_httpContextAccessor.HttpContext?.Request.Method} {_httpContextAccessor.HttpContext?.Request.Path}",
                ResourceId = _httpContextAccessor.HttpContext.GetResourceIdFoRequest(),
                SpanContext = string.Empty,
                TraceId = _traceId.GetTraceId(),
                RequestId = Guid.NewGuid().ToString("N"),
                ConnectionId = _httpContextAccessor.HttpContext?.Connection.Id,
                IdentityContext = new IdentityContext(_httpContextAccessor.HttpContext?.User)
            };

            return context;
        }
    }
}