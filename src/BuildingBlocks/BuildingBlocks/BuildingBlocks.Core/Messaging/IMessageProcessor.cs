using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging
{
    public interface IMessageProcessor
    {
        Task ProcessAsync<TMessage>(TMessage message, IMessageContext messageContext = null, CancellationToken
            cancellationToken = default) where TMessage : IMessage;
    }
}