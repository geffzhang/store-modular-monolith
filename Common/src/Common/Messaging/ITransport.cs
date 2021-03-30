using System.Threading.Tasks;
using Common.Messaging;

namespace Common.Messaging
{
    public interface ITransport
    {
        Task PublishAsync(params IMessage[] messages);
    }
}