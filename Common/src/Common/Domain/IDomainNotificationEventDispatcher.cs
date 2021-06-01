using System.Threading.Tasks;

namespace Common.Domain
{
    public interface IDomainNotificationEventDispatcher
    {
        Task DispatchAsync(params IDomainNotificationEvent[] events);
    }
}