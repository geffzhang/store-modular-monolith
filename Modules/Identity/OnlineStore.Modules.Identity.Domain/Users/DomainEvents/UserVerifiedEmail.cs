using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainEvents
{
    public class UserVerifiedEmail : DomainEventBase
    {
        public User User { get; }

        public UserVerifiedEmail(User user)
        {
            User = user;
        }
    }
}