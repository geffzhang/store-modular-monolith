using System.Threading.Tasks;

namespace Common.Messaging.Commands
{
    public interface ICommandDispatcher
    {
        Task SendAsync<T>(T command) where T : class, ICommand;
    }
}