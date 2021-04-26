using System.Threading.Tasks;
using Common.Messaging.Commands;
using Common.Messaging.Events;
using Common.Messaging.Queries;

namespace Common
{
    public interface IInProcessDispatcher
    {
        Task SendAsync<T>(T command) where T : class, ICommand;
        Task PublishAsync<T>(T @event) where T : class, IEvent;
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
    }
}