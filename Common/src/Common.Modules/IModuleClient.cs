using System.Threading.Tasks;
using Common.Messaging;

namespace Common
{
    public interface IModuleClient
    {
        Task<TResult> RequestAsync<TResult>(string path, object request) where TResult : class;
        Task SendAsync(IMessage message);
    }
}