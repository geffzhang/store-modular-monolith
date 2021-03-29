using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Infrastructure.Messaging
{
    public interface ITransport
    {
        Task PublishAsync(params IMessage[] messages);
    }
}