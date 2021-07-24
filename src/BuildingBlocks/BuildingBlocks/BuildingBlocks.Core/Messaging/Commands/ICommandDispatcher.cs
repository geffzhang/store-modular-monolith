using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging.Commands
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync<T>(T command) where T : class, ICommand;
    }
}