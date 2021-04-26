using Common.Extensions.DependencyInjection;
using Common.Messaging.Scheduling;
using Common.Scheduling.Hangfire.MessagesScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Scheduling.Hangfire
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
            services.AddSingleton<IMessagesScheduler, HangfireMessagesScheduler>();

            return services;
        }
    }
}