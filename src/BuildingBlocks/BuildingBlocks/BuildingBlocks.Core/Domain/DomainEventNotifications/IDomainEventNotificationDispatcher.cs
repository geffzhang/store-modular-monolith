using System.Threading.Tasks;

namespace BuildingBlocks.Core.Domain.DomainEventNotifications
{
    public interface IDomainEventNotificationDispatcher
    {
        Task DispatchAsync<T>(params T[] events) where T : class, IDomainEventNotification;
    }
}