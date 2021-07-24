using BuildingBlocks.Core.Messaging.Commands;
using BuildingBlocks.Core.Scheduling;
using Hangfire;

namespace BuildingBlocks.Messaging.Scheduling.Hangfire.MessagesScheduler
{
    public interface IHangfireMessagesScheduler : IMessagesScheduler
    {
        string Enqueue(ICommand command, string parentJobId, JobContinuationOptions continuationOption,
            string description = null);

        string Enqueue(MessageSerializedObject messageSerializedObject, string parentJobId,
            JobContinuationOptions continuationOption, string description = null);

    }
}