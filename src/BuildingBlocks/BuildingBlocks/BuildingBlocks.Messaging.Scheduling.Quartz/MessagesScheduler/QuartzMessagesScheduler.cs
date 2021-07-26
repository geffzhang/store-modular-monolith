using System;
using System.Threading.Tasks;
using BuildingBlocks.Core.Messaging.Serialization;
using BuildingBlocks.Core.Scheduling;
using BuildingBlocks.Cqrs.Commands;
using Microsoft.Extensions.Options;
using Quartz;

namespace BuildingBlocks.Messaging.Scheduling.Quartz.MessagesScheduler
{
    public class QuartzMessagesScheduler : IQuartzMessagesScheduler
    {
        private readonly IMessageSerializer _messageSerializer;
        private readonly QuartzOptions _options;
        private readonly IScheduler _scheduler;

        public QuartzMessagesScheduler(IScheduler scheduler, IOptions<QuartzOptions> options,
            IMessageSerializer messageSerializer)
        {
            _messageSerializer = messageSerializer;
            _options = options.Value;
            _scheduler = scheduler;
        }

        public Task Enqueue(ICommand command, string description = null)
        {
            var messageSerializedObject = SerializeObject(command, description);
            return Enqueue(messageSerializedObject, description);
        }

        public async Task Enqueue(MessageSerializedObject messageSerializedObject, string description)
        {
            var jobDataMap = new JobDataMap
            {
                {nameof(MessageSerializedObject), _messageSerializer.Serialize(messageSerializedObject)},
                {nameof(_options.RetryCount), _options.RetryCount.ToString()},
                {nameof(_options.RetryIntervalMillisecond), _options.RetryIntervalMillisecond.ToString()},
                {"RetryIndex", "0"}
            };

            var jobDetail = JobBuilder.Create<QuartzDynamicJobCreator<MessageSerializedObject>>().RequestRecovery()
                .WithDescription(description)
                .SetJobData(jobDataMap).Build();
            var trigger = TriggerBuilder.Create().StartNow().Build();
            await _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public async Task Schedule(ICommand command, DateTimeOffset scheduleAt, string description = null)
        {
            var messageSerializedObject = SerializeObject(command, description);
            await Schedule(messageSerializedObject, scheduleAt, description);
        }

        public async Task Schedule(MessageSerializedObject messageSerializedObject, DateTimeOffset scheduleAt,
            string description = null)
        {
            var jobDataMap = new JobDataMap
            {
                {nameof(MessageSerializedObject), _messageSerializer.Serialize(messageSerializedObject)},
                {nameof(_options.RetryCount), _options.RetryCount.ToString()},
                {nameof(_options.RetryIntervalMillisecond), _options.RetryIntervalMillisecond.ToString()},
                {"RetryIndex", "0"}
            };

            var jobDetail = JobBuilder.Create<QuartzDynamicJobCreator<MessageSerializedObject>>().RequestRecovery()
                .WithDescription(description)
                .SetJobData(jobDataMap).Build();
            var trigger = TriggerBuilder.Create().StartAt(scheduleAt).StartNow().Build();

            await _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public async Task Schedule(ICommand command, TimeSpan delay, string description = null)
        {
            var messageSerializedObject = SerializeObject(command, description);
            await Schedule(messageSerializedObject, delay, description);
        }

        public async Task Schedule(MessageSerializedObject messageSerializedObject, TimeSpan delay,
            string description = null)
        {
            var jobDataMap = new JobDataMap
            {
                {nameof(MessageSerializedObject), _messageSerializer.Serialize(messageSerializedObject)},
                {nameof(_options.RetryCount), _options.RetryCount.ToString()},
                {nameof(_options.RetryIntervalMillisecond), _options.RetryIntervalMillisecond.ToString()},
                {"RetryIndex", "0"}
            };

            var jobDetail = JobBuilder.Create<QuartzDynamicJobCreator<MessageSerializedObject>>().RequestRecovery()
                .WithDescription(description)
                .SetJobData(jobDataMap).Build();

            var newTime = DateTime.Now + delay;

            var trigger = TriggerBuilder.Create().StartAt(newTime).Build();

            await _scheduler.ScheduleJob(jobDetail, trigger);
        }

        //https://blog.vfrz.fr/quartz-asp-net-core-3-0
        public async Task ScheduleRecurring(ICommand command, string name, string cronExpression,
            string description = null)
        {
            var messageSerializedObject = SerializeObject(command, description);

            await ScheduleRecurring(messageSerializedObject, name, cronExpression, description);
        }

        public async Task ScheduleRecurring(MessageSerializedObject messageSerializedObject, string name,
            string cronExpression,
            string description = null)
        {
            var jobDataMap = new JobDataMap
            {
                {nameof(MessageSerializedObject), _messageSerializer.Serialize(messageSerializedObject)},
                {nameof(_options.RetryCount), _options.RetryCount.ToString()},
                {nameof(_options.RetryIntervalMillisecond), _options.RetryIntervalMillisecond.ToString()},
                {"RetryIndex", "0"}
            };

            var jobDetail = JobBuilder.Create<QuartzDynamicJobCreator<MessageSerializedObject>>().RequestRecovery()
                .WithDescription(description)
                .WithIdentity(name)
                .SetJobData(jobDataMap).Build();
            var trigger = TriggerBuilder.Create().StartNow().WithCronSchedule("*/5 * * * *").Build();

            await _scheduler.ScheduleJob(jobDetail, trigger);
        }

        private MessageSerializedObject SerializeObject(object messageObject, string description)
        {
            string fullTypeName = messageObject.GetType().FullName;
            string data = _messageSerializer.Serialize(messageObject);

            return new MessageSerializedObject(fullTypeName, data, description);
        }
    }
}