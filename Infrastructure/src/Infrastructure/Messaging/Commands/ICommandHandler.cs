using System.Threading.Tasks;

namespace Infrastructure.Messaging.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : class, ICommand
    {
        Task HandleAsync(TCommand command);
    }
}