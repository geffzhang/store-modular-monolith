using BuildingBlocks.Core.Domain.DomainEvents;
using BuildingBlocks.Core.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Types;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Events
{
    public class UserPasswordChangedDomainEvent : DomainEventBase
    {
        public UserPasswordChangedDomainEvent(UserId userId)
        {
            UserId = userId;
        }

        public UserId UserId { get; }
    }
}