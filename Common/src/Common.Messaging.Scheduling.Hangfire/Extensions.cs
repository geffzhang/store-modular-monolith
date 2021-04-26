using Common.Extensions.DependencyInjection;
using Common.Messaging.Scheduling.Hangfire.MessagesScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Scheduling.Hangfire
{
    public static class Extensions
    {
        public const string SectionName = "hangfire";

        public static IServiceCollection AddHangfireMessageScheduler(IServiceCollection services, string sectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var hangfireOptions = services.GetOptions<HangfireOptions>($"{sectionName}");
            services.AddSingleton(hangfireOptions);

            services.AddSingleton<IHangfireMessagesScheduler, HangfireMessagesScheduler>();

            return services;
        }
    }
}