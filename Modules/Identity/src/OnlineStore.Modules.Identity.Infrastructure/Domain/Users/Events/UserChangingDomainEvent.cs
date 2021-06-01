using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserChangingDomainEvent : DomainEventBase
    {
        public User User { get; }
        public UserChangingDomainEvent(User user)
        {
            User = user;
        }
    }
}