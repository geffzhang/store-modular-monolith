using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Common.Web.Middlewares
{
    public class CorrelationMiddleware: IMiddleware
    {
        internal const string CorrelationHeaderKey = "CorrelationId";

        private readonly RequestDelegate _next;

        public CorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationId = Guid.NewGuid();

            context.Request.Headers.Add(CorrelationHeaderKey, correlationId.ToString());

            await _next.Invoke(context);
        }
    }
}