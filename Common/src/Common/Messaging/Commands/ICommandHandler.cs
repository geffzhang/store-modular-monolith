using System.Threading.Tasks;

namespace Common.Messaging.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand :  ICommand
    {
        Task HandleAsync(TCommand command);
    }
}