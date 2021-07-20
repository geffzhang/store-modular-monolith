using Common.Core.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainEvents
{
    public class UserVerifiedEmailDomainEvent : DomainEventBase
    {
        public User User { get; }

        public UserVerifiedEmailDomainEvent(User user)
        {
            User = user;
        }
    }
}