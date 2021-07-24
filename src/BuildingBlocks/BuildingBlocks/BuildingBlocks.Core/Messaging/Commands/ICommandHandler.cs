using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand :  ICommand
    {
        Task HandleAsync(TCommand command);
    }
}