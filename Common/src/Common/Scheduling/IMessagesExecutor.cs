using System.Threading.Tasks;

namespace Common.Messaging.Scheduling
{
    public interface IMessagesExecutor
    {
        Task ExecuteCommand(MessageSerializedObject messageSerializedObject);
    }
}