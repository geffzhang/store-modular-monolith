using System.Threading.Tasks;
using Common.Core.Messaging.Commands;

namespace Common.Core.Scheduling
{
    public interface IEnqueueMessages
    {
        Task Enqueue(ICommand command, string description = null);
        Task Enqueue(MessageSerializedObject messageSerializedObject, string description = null);
    }
}