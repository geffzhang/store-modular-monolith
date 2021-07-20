using System.Threading.Tasks;

namespace Common.Core.Messaging
{
    public interface IMessageHandler<in TMessage> where TMessage : IMessage
    {
        Task HandleAsync(TMessage message);
    }
}