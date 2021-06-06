using System.Threading.Tasks;
using Common.Messaging;
using Common.Messaging.Events;

namespace Common.Modules
{
    public interface IModuleClient
    {
        Task SendAsync(string path, object request);
        Task<TResult> SendAsync<TResult>(string path, object request) where TResult : class;
        Task PublishAsync(IMessage message);
    }
}
