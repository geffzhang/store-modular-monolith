using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Extensions.DependencyInjection;
using Infrastructure.BackgroundServices;

namespace Infrastructure.Messaging.Transport.InMemory
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