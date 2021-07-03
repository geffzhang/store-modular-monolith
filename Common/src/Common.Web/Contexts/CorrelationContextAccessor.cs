using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utils.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Common.Web.Contexts
{
    public class CorrelationContextAccessor : ICorrelationContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CorrelationContextOptions _options;
        private readonly Dictionary<string, string> _context = new();

        public CorrelationContextAccessor(IHttpContextAccessor httpContextAccessor,
            IOptions<CorrelationContextOptions> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options.Value;
            _context[_options?.CorrelationIdHeaderKey ?? "x-correlation-id"] =
                httpContextAccessor.HttpContext?.Request.Headers
                    .Get<string>(_options?.CorrelationIdHeaderKey ??"x-correlation-id") ?? Guid.NewGuid().ToString();

            var correlationHeaders = httpContextAccessor.HttpContext?.Request.Headers
                .Where(h => h.Key.ToLowerInvariant().StartsWith("x-correlation-"))
                .ToDictionary(h => h.Key, h => (object) h.Value.ToString());
            if (correlationHeaders is not null)
            {
                foreach (var correlationHeader in correlationHeaders)
                {
                    Update(correlationHeader.Key, correlationHeader.Value.ToString());
                }
            }
        }

        public CorrelationContext CorrelationContext
        {
            get
            {
                var contextBody = _httpContextAccessor.HttpContext?.Request.Headers.Get<string>(
                    _options?.CorrelationContextHeaderKey ?? "correlation-context");
                if (contextBody is not null)
                    return JsonConvert.DeserializeObject<CorrelationContext>(contextBody);

                return new CorrelationContext
                {
                    CorrelationId = _context[_options?.CorrelationIdHeaderKey ?? "x-correlation-id"]
                };
            }
        }

        private void Update(string key, string value)
        {
            _context[key] = value;
        }

        public IReadOnlyDictionary<string, object> GetCorrelationContextValues()
        {
            return _context.ToDictionary(k => k.Key, k => (object) k.Value);
        }
    }
}