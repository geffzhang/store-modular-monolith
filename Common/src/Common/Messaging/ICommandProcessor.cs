using System.Threading.Tasks;
using Common.Messaging.Commands;
using Common.Messaging.Events;

namespace Common.Messaging
{
    public interface ICommandProcessor
    {
        Task SendCommandAsync<T>(T command) where T : class, ICommand;
        Task PublishEventAsync<T>(T @event) where T : class, IEvent;
        Task PublishMessageAsync<T>(T message) where T : class, IMessage;
    }
}