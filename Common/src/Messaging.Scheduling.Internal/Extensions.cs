using Common.Core.Scheduling;
using Common.Messaging.Scheduling.Internal.MessagesScheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Scheduling.Internal
{
    public static class Extensions
    {
        public const string SectionName = "internal-scheduler";

        public static IServiceCollection AddInternalMessageScheduler(IServiceCollection services,IConfiguration configuration,
            string sectionName = null)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;
            
            var options = configuration.GetSection(sectionName).Get<InternalMessageOptions>();
            services.AddOptions<InternalMessageOptions>().Bind(configuration.GetSection(sectionName)).ValidateDataAnnotations();
            
            services.AddSingleton<IEnqueueMessages, InternalMessageScheduler>();

            return services;
        }
    }
}