using System.Threading.Tasks;

namespace Common.Domain
{
    public interface IDomainNotificationEventHandler<in TDomainNotificationEvent>
        where TDomainNotificationEvent : class, IDomainNotificationEvent
    {
        Task HandleAsync(TDomainNotificationEvent @event);
    }
}