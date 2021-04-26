using System;
using System.Threading.Tasks;
using Common.Messaging.Commands;
using Common.Messaging.Scheduling;
using Hangfire;
using Newtonsoft.Json;

namespace Common.Scheduling.Hangfire.MessagesScheduler
{
    public class HangfireMessagesScheduler : IHangfireMessagesScheduler
    {
        private readonly IMessagesExecutor _messagesExecutor;

        public HangfireMessagesScheduler(IMessagesExecutor messagesExecutor)
        {
            _messagesExecutor = messagesExecutor;
        }


        public Task<string> Enqueue(ICommand command, string description)
        {
            var messageSerializedObject = SerializeObject(command, description);
            return Task.FromResult(
                BackgroundJob.Enqueue(() => _messagesExecutor.ExecuteCommand(messageSerializedObject)));
        }

        public Task<string> Enqueue(MessageSerializedObject messageSerializedObject, string description = null)
        {
            return Task.FromResult(
                BackgroundJob.Enqueue(() => _messagesExecutor.ExecuteCommand(messageSerializedObject)));
        }

        public string Enqueue(ICommand command, string parentJobId, JobContinuationOptions continuationOption,
            string description = null)
        {
            var messageSerializedObject = SerializeObject(command, description);

            return BackgroundJob.ContinueJobWith(parentJobId,
                () => _messagesExecutor.ExecuteCommand(messageSerializedObject), continuationOption);
        }

        public string Enqueue(MessageSerializedObject messageSerializedObject, string parentJobId,
            JobContinuationOptions continuationOption, string description = null)
        {
            return BackgroundJob.ContinueJobWith(parentJobId,
                () => _messagesExecutor.ExecuteCommand(messageSerializedObject), continuationOption);
        }

        public Task Schedule(ICommand command, DateTimeOffset scheduleAt, string description = null)
        {
            var mediatorSerializedObject = SerializeObject(command, description);
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(mediatorSerializedObject), scheduleAt);

            return Task.CompletedTask;
        }

        public Task Schedule(MessageSerializedObject mediatorSerializedObject, DateTimeOffset scheduleAt,
            string description = null)
        {
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(mediatorSerializedObject), scheduleAt);

            return Task.CompletedTask;
        }

        public Task Schedule(ICommand command, TimeSpan delay, string description = null)
        {
            var mediatorSerializedObject = SerializeObject(command, description);
            var newTime = DateTime.Now + delay;
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(mediatorSerializedObject), newTime);
            
            return Task.CompletedTask;
        }

        public Task Schedule(MessageSerializedObject messageSerializedObject, TimeSpan delay,
            string description = null)
        {
            var newTime = DateTime.Now + delay;
            BackgroundJob.Schedule(() => _messagesExecutor.ExecuteCommand(messageSerializedObject), newTime);
            
            return Task.CompletedTask;
        }

        public Task ScheduleRecurring(ICommand command, string name, string cronExpression, string description = null)
        {
            var mediatorSerializedObject = SerializeObject(command, description);
            RecurringJob.AddOrUpdate(name, () => _messagesExecutor.ExecuteCommand(mediatorSerializedObject),
                cronExpression, TimeZoneInfo.Local);
            
            return Task.CompletedTask;
        }

        public Task ScheduleRecurring(MessageSerializedObject messageSerializedObject, string name,
            string cronExpression, string description = null)
        {
            RecurringJob.AddOrUpdate(name, () => _messagesExecutor.ExecuteCommand(messageSerializedObject),
                cronExpression, TimeZoneInfo.Local);
            
            return Task.CompletedTask;
        }

        private MessageSerializedObject SerializeObject(object messageObject, string description)
        {
            string fullTypeName = messageObject.GetType().FullName;
            string data = JsonConvert.SerializeObject(messageObject, new JsonSerializerSettings
            {
                Formatting = Formatting.None,
            });

            return new MessageSerializedObject(fullTypeName, data, description);
        }
    }
}