using System.Threading.Tasks;

namespace BuildingBlocks.Core.Messaging
{
    public interface IMessageHandler<in TMessage> where TMessage : IMessage
    {
        Task HandleAsync(TMessage message);
    }
}