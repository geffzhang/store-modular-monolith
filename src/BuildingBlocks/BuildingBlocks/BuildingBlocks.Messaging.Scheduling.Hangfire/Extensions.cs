using BuildingBlocks.Messaging.Scheduling.Hangfire.MessagesScheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Scheduling.Hangfire
{
    public static class Extensions
    {
        public const string SectionName = "hangfire";

        public static IServiceCollection AddHangfireMessageScheduler(IServiceCollection services, IConfiguration configuration,
            string sectionName = SectionName)
        {
            var options = configuration.GetSection(sectionName).Get<HangfireOptions>();
            services.AddOptions<HangfireOptions>().Bind(configuration.GetSection(sectionName)).ValidateDataAnnotations();

            services.AddSingleton<IHangfireMessagesScheduler, HangfireMessagesScheduler>();

            return services;
        }
    }
}