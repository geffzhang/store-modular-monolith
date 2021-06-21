using Common.Persistence.MSSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Outbox.EFCore
{
    public static class Extensions
    {
        private const string SectionName = "messaging";

        public static IServiceCollection AddEntityFrameworkOutbox<TContext>(this IServiceCollection services, IConfiguration configuration,
            string sectionName = SectionName)
            where TContext : DbContext, ISqlDbContext
        {
            var outboxOptions = configuration.GetSection("messaging:outbox").Get<OutboxOptions>();
            services.AddOptions<OutboxOptions>().Bind(configuration.GetSection("messaging:outbox")).ValidateDataAnnotations();

            services.AddScoped<IOutbox, EFCoreOutbox<TContext>>();

            // Adding background service
            if (outboxOptions.Enabled)
                services.AddHostedService<OutboxProcessor>();

            return services;
        }
    }
}