using Common.Core.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserChangedDomainEvent : DomainEventBase
    {
        public User User { get; }

        public UserChangedDomainEvent(User user)
        {
            User = user;
        }
    }
}