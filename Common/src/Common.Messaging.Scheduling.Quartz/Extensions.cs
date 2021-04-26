using System.Collections.Specialized;
using Common.Extensions.DependencyInjection;
using Common.Messaging.Scheduling.Quartz.MessagesScheduler;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;

namespace Common.Messaging.Scheduling.Quartz
{
    public static class Extensions
    {
        public const string SectionName = "quartz";

        public static IServiceCollection AddQuartzMessageScheduler(IServiceCollection services,
            string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var quartzOptions = services.GetOptions<QuartzOptions>($"{sectionName}");
            services.AddSingleton(quartzOptions);

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