using System;
using Microsoft.AspNetCore.Http;

namespace Common.Web.Contexts
{
    public class ExecutionContextFactory : IExecutionContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExecutionContextFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ExecutionContext Create()
        {
            var context = new ExecutionContext
            {
                Name = $"{_httpContextAccessor.HttpContext?.Request.Method} {_httpContextAccessor.HttpContext?.Request.Path}",
                ResourceId = _httpContextAccessor.HttpContext.GetResourceIdFoRequest(),
                SpanContext = string.Empty,
                TraceId = _httpContextAccessor.HttpContext?.TraceIdentifier,
                RequestId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N"),
                ConnectionId = _httpContextAccessor.HttpContext?.Connection.Id,
                IdentityContext = new IdentityContext(_httpContextAccessor.HttpContext?.User)
            };

            return context;
        }
    }
}