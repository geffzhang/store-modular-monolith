using System.Threading.Tasks;

namespace Common.Scheduling
{
    public interface IMessagesExecutor
    {
        Task ExecuteCommand(MessageSerializedObject messageSerializedObject);
    }
}