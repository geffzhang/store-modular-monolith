using System.Threading.Tasks;

namespace BuildingBlocks.Core.Scheduling
{
    public interface IMessagesExecutor
    {
        Task ExecuteCommand(MessageSerializedObject messageSerializedObject);
    }
}