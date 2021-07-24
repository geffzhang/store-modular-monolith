using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging
{
    public interface IMessageDispatcher
    {
        Task DispatchAsync<T>(T message) where T : class, IMessage;
    }
}