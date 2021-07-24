using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.Contexts
{
    public class HttpTraceId : IHttpTraceId
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpTraceId(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTraceId()
        {
            var currentActivity = Activity.Current;
            if (currentActivity != null)
            {
                return currentActivity.TraceId.ToString();
            }
            
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("traceparent", out var traceId))
            {
                return traceId.ToString();
            }

            if (_httpContextAccessor.HttpContext != null && !string.IsNullOrEmpty(_httpContextAccessor.HttpContext.TraceIdentifier))
            {
                return _httpContextAccessor.HttpContext.TraceIdentifier;
            }

            return null;
        }
    }
}