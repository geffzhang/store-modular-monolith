using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Common.Web.Contexts.Middleware
{
    public class LogContextMiddleware : IMiddleware
    {
        private readonly IExecutionContextAccessor _executionContextAccessor;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public LogContextMiddleware(IExecutionContextAccessor executionContextAccessor,
            ICorrelationContextAccessor correlationContextAccessor)
        {
            _executionContextAccessor = executionContextAccessor;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // ensures all entries are tagged with some values
            using (LogContext.PushProperty("Correlation-Context", _correlationContextAccessor.CorrelationContext))
            {
                using (LogContext.PushProperty("ExecutionContext", _executionContextAccessor.ExecutionContext))
                {
                    // Call the next delegate/middleware in the pipeline
                    return next(context);
                }
            }
        }
    }
}