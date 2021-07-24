using System.Threading.Tasks;

namespace BuildingBlocks.Core.Domain.Dispatching
{
    public interface IDomainEventNotificationDispatcher
    {
        Task DispatchAsync<T>(params T[] events) where T : class, IDomainEventNotification;
    }
}