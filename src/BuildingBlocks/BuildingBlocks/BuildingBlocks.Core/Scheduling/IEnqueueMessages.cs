using System.Threading.Tasks;
using BuildingBlocks.Core.Messaging.Commands;

namespace BuildingBlocks.Core.Scheduling
{
    public interface IEnqueueMessages
    {
        Task Enqueue(ICommand command, string description = null);
        Task Enqueue(MessageSerializedObject messageSerializedObject, string description = null);
    }
}