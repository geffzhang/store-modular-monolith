using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserCreatedDomainEvent : DomainEventBase
    {
        public User User { get; }

        public UserCreatedDomainEvent(User user)
        {
            User = user;
        }
    }
}