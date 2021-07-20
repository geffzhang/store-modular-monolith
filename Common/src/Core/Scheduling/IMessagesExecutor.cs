using System.Threading.Tasks;

namespace Common.Core.Scheduling
{
    public interface IMessagesExecutor
    {
        Task ExecuteCommand(MessageSerializedObject messageSerializedObject);
    }
}