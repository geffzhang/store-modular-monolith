using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BuildingBlocks.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace BuildingBlocks.Tests.Integration.Fixtures
{
    public abstract class WebApiTestBase<TEntryPoint> : IDisposable, IClassFixture<WebApplicationFactory<TEntryPoint>>,
        IClassFixture<MongoFixture>
        where TEntryPoint : class
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {new JsonStringEnumConverter()}
        };

        private string _route;

        protected void SetPath(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                _route = string.Empty;
                return;
            }

            if (route.StartsWith("/"))
            {
                route = route.Substring(1, route.Length - 1);
            }

            if (route.EndsWith("/"))
            {
                route = route.Substring(0, route.Length - 1);
            }

            _route = $"{route}/";
        }

        protected static T Map<T>(object data) =>
            JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(data, SerializerOptions), SerializerOptions);

        protected Task<HttpResponseMessage> GetAsync(string endpoint)
            => Client.GetAsync(GetEndpoint(endpoint));

        protected async Task<T> GetAsync<T>(string endpoint)
            => await ReadAsync<T>(await GetAsync(endpoint));

        protected Task<HttpResponseMessage> PostAsync<T>(string endpoint, T command)
            => Client.PostAsync(GetEndpoint(endpoint), GetPayload(command));

        protected Task<HttpResponseMessage> PutAsync<T>(string endpoint, T command)
            => Client.PutAsync(GetEndpoint(endpoint), GetPayload(command));

        protected Task<HttpResponseMessage> DeleteAsync(string endpoint)
            => Client.DeleteAsync(GetEndpoint(endpoint));

        protected Task<HttpResponseMessage> SendAsync(string method, string endpoint)
            => SendAsync(GetMethod(method), endpoint);

        protected Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint)
            => Client.SendAsync(new HttpRequestMessage(method, GetEndpoint(endpoint)));

        protected void Authenticate(Guid userId, string email, string userName)
        {
            var jwt = AuthHelper.GenerateJwt(userName, email, userId.ToString());
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        }

        private static HttpMethod GetMethod(string method)
            => method.ToUpperInvariant() switch
            {
                "GET" => HttpMethod.Get,
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                "DELETE" => HttpMethod.Delete,
                _ => null
            };

        private string GetEndpoint(string endpoint) => $"{_route}{endpoint}";

        private static StringContent GetPayload(object value)
            => new(JsonSerializer.Serialize(value, SerializerOptions), Encoding.UTF8, "application/json");

        protected static async Task<T> ReadAsync<T>(HttpResponseMessage response)
            => JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), SerializerOptions);

        #region Arrange

        protected readonly HttpClient Client;
        protected readonly MongoFixture Mongo;
        private readonly WebApplicationFactory<Program> _factory;

        protected WebApiTestBase(WebApplicationFactory<Program> factory, MongoFixture mongo,
            string environment = "test")
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(environment);
                builder.ConfigureServices(ConfigureServices);
            });
            Client = _factory.CreateClient();
            Mongo = mongo;
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void Dispose()
        {
        }

        #endregion
    }
}