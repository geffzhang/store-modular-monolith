using System.Threading.Tasks;

namespace Common.Core.Messaging
{
    public interface IMessageDispatcher
    {
        Task DispatchAsync<T>(T message) where T : class, IMessage;
    }
}