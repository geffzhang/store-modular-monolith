using System.Threading.Tasks;

namespace Common.Messaging.Transport
{
    public interface ITransport
    {
        Task PublishAsync(params IMessage[] messages);
    }
}