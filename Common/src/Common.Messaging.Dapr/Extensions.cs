using System.Text.Json;
using Common.Messaging.Transport;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using RestEase.HttpClientFactory;
using System;
using Common.Messaging.Dapr.Outbox;
using Common.Messaging.Outbox;

namespace Common.Messaging.Dapr
{
    public static class Extensions
    {
        private const string SectionName = "dapr";

        public static IServiceCollection AddDaprMessageBroker(this IServiceCollection services, IConfiguration configuration,
            string sectionName = SectionName)
        {
            services.AddDaprClient();
            services.AddScoped<ITransport, DaprTransport>();

            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var daprOptions = configuration.GetSection(sectionName).Get<DaprEventBusOptions>();
            var outboxOptions = configuration.GetSection("messaging:outbox").Get<OutboxOptions>();

            services.AddOptions<DaprEventBusOptions>().Bind(configuration.GetSection(sectionName)).ValidateDataAnnotations();
            services.AddOptions<OutboxOptions>().Bind(configuration.GetSection("messaging:outbox")).ValidateDataAnnotations();
            
            services
                .AddTransient<IOutbox, DaprOutbox>();

            // Adding background service
            if (outboxOptions.Enabled)
                services.AddHostedService<OutboxProcessor>();

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

        public static IServiceCollection AddRestClient(this IServiceCollection services, Type httpClientApi, string appName = "localhost",
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