using Common.Messaging.Commands;
using Common.Messaging.Scheduling;
using Hangfire;

namespace Common.Scheduling.Hangfire.MessagesScheduler
{
    public interface IHangfireMessagesScheduler : IMessagesScheduler
    {
        string Enqueue(ICommand command, string parentJobId, JobContinuationOptions continuationOption,
            string description = null);

        string Enqueue(MessageSerializedObject messageSerializedObject, string parentJobId,
            JobContinuationOptions continuationOption, string description = null);

    }
}