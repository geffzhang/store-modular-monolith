using System.Net.Http;
using System.Net.Http.Headers;
using Common.Tests.Integration.Constants;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Common.Tests.Integration.Extensions
{
    public static class WebApplicationFactoryExtensions
    {
        public static HttpClient CreateClientWithTestAuth<T>(this WebApplicationFactory<T> factory) where T : class
        {
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthConstants.Scheme);

            return client;
        }
    }
}