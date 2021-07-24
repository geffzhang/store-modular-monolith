using BuildingBlocks.Core.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Aggregates.Users.DomainEvents
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