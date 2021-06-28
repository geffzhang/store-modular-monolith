using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainEvents
{
    public class NewUserRegisteredDomainEvent : DomainEventBase
    {
        public NewUserRegisteredDomainEvent(User user)
        {
            User = user;
        }

        public User User { get; set;}
    }
}