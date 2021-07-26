using BuildingBlocks.Core.Domain.DomainEvents;
using BuildingBlocks.Core.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Events
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