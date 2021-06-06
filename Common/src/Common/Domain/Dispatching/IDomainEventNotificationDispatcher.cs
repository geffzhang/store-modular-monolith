using System.Threading.Tasks;

namespace Common.Domain.Dispatching
{
    public interface IDomainEventNotificationDispatcher
    {
        Task DispatchAsync(params IDomainEventNotification[] events);
    }
}