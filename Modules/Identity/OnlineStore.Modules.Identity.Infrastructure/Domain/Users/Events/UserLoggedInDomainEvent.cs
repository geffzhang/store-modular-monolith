using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserLoggedInDomainEvent : DomainEventBase
    {
        public User User { get;  }

        public UserLoggedInEvent(User user)
        {
            User = user;
        }
    }
}
