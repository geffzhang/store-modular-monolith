using System.Threading.Tasks;

namespace BuildingBlocks.Core.Domain.DomainEventNotifications
{
    public interface IDomainEventNotificationHandler<in T> where T : class, IDomainEventNotification
    {
        Task HandleAsync(T domainEvent);
    }
}