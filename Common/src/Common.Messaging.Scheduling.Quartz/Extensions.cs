using System.Collections.Specialized;
using Common.Messaging.Scheduling.Quartz.MessagesScheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;

namespace Common.Messaging.Scheduling.Quartz
{
    public static class Extensions
    {
        public const string SectionName = "quartz";

        public static IServiceCollection AddQuartzMessageScheduler(IServiceCollection services, IConfiguration configuration,
            string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var quartzOptions = configuration.GetSection(sectionName).Get<QuartzOptions>();
            services.AddOptions<QuartzOptions>().Bind(configuration.GetSection(sectionName)).ValidateDataAnnotations();

            var schedulerConfiguration = new NameValueCollection
            {
                {"quartz.scheduler.instanceName", quartzOptions.InstanceName}
            };

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory(schedulerConfiguration);
            IScheduler scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            scheduler.Start().GetAwaiter().GetResult();

            services.AddSingleton(scheduler);

            services.AddSingleton<IQuartzMessagesScheduler, QuartzMessagesScheduler>();

            return services;
        }
    }
}