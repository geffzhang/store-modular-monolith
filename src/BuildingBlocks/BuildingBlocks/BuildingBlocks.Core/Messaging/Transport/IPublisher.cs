using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging.Transport
{
    public interface IPublisher
    {
        Task PublishAsync<T>(T message) where T : class, IMessage;
    }
}