using Common.Core.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Events
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
