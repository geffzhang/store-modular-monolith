using System.Threading.Tasks;
using Infrastructure.Messaging;

namespace Infrastructure
{
    public interface IModuleClient
    {
        Task<TResult> RequestAsync<TResult>(string path, object request) where TResult : class;
        Task SendAsync(IMessage message);
    }
}
