using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging
{
    public delegate Task HandleMessageDelegate<in TMessage>(TMessage request, IMessageContext messageContext = null,
        CancellationToken cancellationToken = default);
}