using System.Net.Http;
using Polly;
using Polly.Extensions.Http;

namespace BuildingBlocks.Web.Http.Polly
{
    public static class HttpPolicyBuilders
    {
        public static PolicyBuilder<HttpResponseMessage> GetBaseBuilder()
        {
            return HttpPolicyExtensions.HandleTransientHttpError();
        }
    }
}