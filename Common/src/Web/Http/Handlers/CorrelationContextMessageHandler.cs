using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Web.Contexts;
using Newtonsoft.Json;

namespace Common.Web.Http.Handlers
{
    //https://www.stevejgordon.co.uk/httpclientfactory-aspnetcore-outgoing-request-middleware-pipeline-delegatinghandlers
    public class CorrelationContextMessageHandler : DelegatingHandler
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public CorrelationContextMessageHandler(ICorrelationContextAccessor correlationContextAccessor)
        {
            _correlationContextAccessor = correlationContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddCorrelationHeaders(request);
            return base.SendAsync(request, cancellationToken);
        }

        private void AddCorrelationHeaders(HttpRequestMessage request)
        {
            foreach (var (key, value) in _correlationContextAccessor.GetCorrelationContextValues())
            {
                request.Headers.Add(key, value.ToString());
            }

            request.Headers.TryAddWithoutValidation("Correlation-Context",
                JsonConvert.SerializeObject(_correlationContextAccessor.CorrelationContext));
        }
    }
}