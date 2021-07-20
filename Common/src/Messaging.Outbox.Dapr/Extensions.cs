using Common.Core.Messaging.Outbox;
using Common.Messaging.Outbox.EFCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Outbox.Dapr
{
    public static class Extensions
    {
        private const string SectionName = "messaging";

        public static IServiceCollection AddDaprOutbox(this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = SectionName)
        {
            var outboxOptions = configuration.GetSection("messaging:outbox").Get<OutboxOptions>();
            services.AddOptions<OutboxOptions>().Bind(configuration.GetSection("messaging:outbox"))
                .ValidateDataAnnotations();

            services.AddScoped<IOutbox, DaprOutbox>();

            // Adding background service
            if (outboxOptions.Enabled)
                services.AddHostedService<OutboxProcessorBackgroundService>();

            return services;
        }
    }
}