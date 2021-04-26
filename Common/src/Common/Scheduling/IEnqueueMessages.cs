using System.Threading.Tasks;
using Common.Messaging.Commands;

namespace Common.Messaging.Scheduling
{
    public interface IEnqueueMessages
    {
        Task<string> Enqueue(ICommand command, string description = null);
        Task<string> Enqueue(MessageSerializedObject messageSerializedObject, string description = null);
    }
}