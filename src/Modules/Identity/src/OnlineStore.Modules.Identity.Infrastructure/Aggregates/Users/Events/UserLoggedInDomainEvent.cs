using BuildingBlocks.Core.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Events
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
