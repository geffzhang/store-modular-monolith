using Infrastructure.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Messaging.Outbox.Mongo
{
    internal static class Extensions
    {
        private const string SectionName = "messaging";

        public static IServiceCollection AddMongoOutbox(this IServiceCollection services,
            string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }

            var outboxOptions = services.GetOptions<OutboxOptions>($"{sectionName}:outbox");

            services
                .AddSingleton(outboxOptions)
                .AddTransient<IOutbox, MongoOutbox>();

            if (outboxOptions.Enabled)
            {
                services.AddHostedService<OutboxProcessor>();
            }

            return services;
        }
    }
}