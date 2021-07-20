using System.Threading.Tasks;

namespace Common.Core.Domain.Dispatching
{
    public interface IDomainEventNotificationDispatcher
    {
        Task DispatchAsync(params IDomainEventNotification[] events);
    }
}