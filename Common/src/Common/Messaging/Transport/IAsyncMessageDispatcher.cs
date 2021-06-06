using System.Threading.Tasks;
using Common.Messaging.Events;

namespace Common.Messaging.Transport
{
    public interface IAsyncMessageDispatcher
    {
        Task PublishAsync<T>(T message) where T : class, IMessage;
    }
}