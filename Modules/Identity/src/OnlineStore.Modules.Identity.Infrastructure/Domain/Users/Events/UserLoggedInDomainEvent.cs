using Common.Core.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserLoggedInDomainEvent : DomainEventBase
    {
        public User User { get;  }

        public UserLoggedInDomainEvent(User user)
        {
            User = user;
        }
    }
}
