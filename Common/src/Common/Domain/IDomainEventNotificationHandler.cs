using System.Threading.Tasks;

namespace Common.Domain
{
    public interface IDomainEventNotificationHandler<in T> where T : class, IDomainEventNotification
    {
        Task HandleAsync(T domainEvent);
    }
}