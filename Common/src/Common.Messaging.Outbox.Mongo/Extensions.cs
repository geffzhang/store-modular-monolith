using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Outbox.Mongo
{
    public static class Extensions
    {
        private const string SectionName = "messaging";

        public static IServiceCollection AddMongoOutbox(this IServiceCollection services, IConfiguration configuration,
            string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var options = configuration.GetSection($"{sectionName}:outbox").Get<OutboxOptions>();
            services.AddOptions<OutboxOptions>().Bind(configuration.GetSection($"{sectionName}:outbox")).ValidateDataAnnotations();

            services.AddTransient<IOutbox, MongoOutbox>();

            // Adding background service
            if (options.Enabled)
                services.AddHostedService<OutboxProcessor>();

            return services;
        }
    }
}