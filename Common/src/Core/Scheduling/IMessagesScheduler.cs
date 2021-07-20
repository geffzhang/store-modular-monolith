using System;
using System.Threading.Tasks;
using Common.Core.Messaging.Commands;

namespace Common.Core.Scheduling
{
    public interface IMessagesScheduler: IEnqueueMessages
    {
        Task Schedule(ICommand command, DateTimeOffset scheduleAt, string description = null);
        Task Schedule(MessageSerializedObject messageSerializedObject, DateTimeOffset scheduleAt,
            string description = null);
        Task Schedule(ICommand command, TimeSpan delay, string description = null);
        Task Schedule(MessageSerializedObject messageSerializedObject, TimeSpan delay, string description = null);
        Task ScheduleRecurring(ICommand request, string name, string cronExpression, string description = null);
        Task ScheduleRecurring(MessageSerializedObject messageSerializedObject, string name, string cronExpression,
            string description = null);
    }
}