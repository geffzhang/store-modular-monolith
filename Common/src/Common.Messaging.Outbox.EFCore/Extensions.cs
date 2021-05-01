using Common.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Outbox.EFCore
{
    public static class Extensions
    {
        private const string SectionName = "messaging";

        public static IServiceCollection AddEntityFrameworkOutbox<T>(this IServiceCollection services,
            string sectionName = SectionName) where T : DbContext
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var outboxOptions = services.GetOptions<OutboxOptions>($"{sectionName}:outbox");

            services.AddDbContext<T>();

            services
                .AddSingleton(outboxOptions)
                .AddTransient<IOutbox, EFCoreOutbox<T>>();

            // Adding background service
            if (outboxOptions.Enabled)
                services.AddHostedService<OutboxProcessor>();

            return services;
        }
    }
}