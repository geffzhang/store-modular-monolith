using System.Threading.Tasks;
using Common.Messaging.Events;

namespace Common.Messaging.Transport
{
    public interface ITransport
    {
        Task PublishAsync(params IMessage[] messages);
    }
}