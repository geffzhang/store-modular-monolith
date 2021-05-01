using System.Threading.Tasks;

namespace Common.Messaging.Transport
{
    public interface IAsyncMessageDispatcher
    {
        Task PublishAsync<T>(T message) where T : class, IMessage;
    }
}