using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging
{
    public interface IMessageHandler<in TMessage> where TMessage : IMessage
    {
        Task HandleAsync(TMessage message, IMessageContext messageContext = null,
            CancellationToken cancellationToken = default);
    }
}