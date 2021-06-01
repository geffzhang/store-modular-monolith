using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserLoggedOutDomainEvent : DomainEventBase
    {
        public UserLoggedOutDomainEvent(User user)
        {
            User = user;
        }

        public User User { get; set; }
    }
}
