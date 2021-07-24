using BuildingBlocks.Core.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Events
{
    public class UserChangingDomainEvent : DomainEventBase
    {
        public User User { get; }
        public UserChangingDomainEvent(User user)
        {
            User = user;
        }
    }
}