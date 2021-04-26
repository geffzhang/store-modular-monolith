using Common.Extensions.DependencyInjection;
using Common.Messaging.Scheduling.Internal.MessagesScheduler;
using Common.Scheduling;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Scheduling.Internal
{
    public static class Extensions
    {
        public const string SectionName = "internal-scheduler";

        public static IServiceCollection AddInternalMessageScheduler(IServiceCollection services,
            string sectionName = null)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var hangfireOptions = services.GetOptions<InternalMessageOptions>($"{sectionName}");
            services.AddSingleton(hangfireOptions);

            services.AddSingleton<IEnqueueMessages, InternalMessageScheduler>();

            return services;
        }
    }
}