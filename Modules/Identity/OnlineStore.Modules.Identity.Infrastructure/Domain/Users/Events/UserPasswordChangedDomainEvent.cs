using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Users.Types;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
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