using Microsoft.Extensions.DependencyInjection;
using Common.Extensions.DependencyInjection;
using Common.BackgroundServices;

namespace Common.Messaging.Transport.InMemory
{
    internal static class Extensions
    {
        private const string SectionName = "messaging";

        public static IServiceCollection AddInMemoryMessageBroker(this IServiceCollection services,
            string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }

            var messagingOptions = services.GetOptions<MessagingOptions>(sectionName);
            services
                .AddSingleton(messagingOptions)
                .AddScoped<ITransport, InMemoryMessageBroker>();

            if (messagingOptions.UseBackgroundDispatcher)
            {
                services.AddHostedService<BackgroundDispatcher>();
            }

            return services;
        }
    }
}