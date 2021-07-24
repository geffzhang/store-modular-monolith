using BuildingBlocks.Core.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Aggregates.Users.DomainEvents
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