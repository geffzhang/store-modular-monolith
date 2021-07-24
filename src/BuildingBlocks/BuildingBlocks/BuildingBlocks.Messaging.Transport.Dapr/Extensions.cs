using System;
using System.Text.Json;
using BuildingBlocks.Core.Messaging.Outbox;
using BuildingBlocks.Core.Messaging.Transport;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using RestEase.HttpClientFactory;

namespace BuildingBlocks.Messaging.Transport.Dapr
{
    public static class Extensions
    {
        private const string SectionName = "dapr";

        public static IServiceCollection AddDaprMessageBroker(this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = SectionName)
        {
            services.AddDaprClient();
            services.AddScoped<ITransport, DaprTransport>();

            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var daprOptions = configuration.GetSection(sectionName).Get<DaprEventBusOptions>();
            var outboxOptions = configuration.GetSection("messaging:outbox").Get<OutboxOptions>();

            services.AddOptions<DaprEventBusOptions>().Bind(configuration.GetSection(sectionName))
                .ValidateDataAnnotations();
            services.AddOptions<OutboxOptions>().Bind(configuration.GetSection("messaging:outbox"))
                .ValidateDataAnnotations();

            services.AddHostedService<SubscribersBackgroundService>();

            return services;
        }

        public static IServiceCollection AddDaprClient(this IServiceCollection services)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true,
            };

            services.AddSingleton(options);

            services.AddDaprClient(client => { client.UseJsonSerializationOptions(options); });

            return services;
        }

        public static IServiceCollection AddRestClient(this IServiceCollection services, Type httpClientApi,
            string appName = "localhost",
            int appPort = 5000)
        {
            var appUri = $"http://{appName}:{appPort}";

            services.AddScoped<InvocationHandler>();
            services.AddRestEaseClient(httpClientApi, appUri,
                    client => { client.RequestPathParamSerializer = new StringEnumRequestPathParamSerializer(); })
                .AddHttpMessageHandler<InvocationHandler>();

            return services;
        }
    }
}