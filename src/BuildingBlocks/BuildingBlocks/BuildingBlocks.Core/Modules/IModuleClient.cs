using System.Threading.Tasks;
using BuildingBlocks.Core.Messaging;

namespace BuildingBlocks.Core.Modules
{
    public interface IModuleClient
    {
        Task SendAsync(string path, object request);
        Task<TResult> SendAsync<TResult>(string path, object request) where TResult : class;
        Task PublishAsync(IMessage message);
    }
}
