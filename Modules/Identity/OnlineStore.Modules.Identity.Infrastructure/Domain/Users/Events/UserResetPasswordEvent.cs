using Common.Domain.Types;
using OnlineStore.Modules.Identity.Domain.Users.Types;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserResetPasswordDomainEvent : DomainEventBase
    {
        public UserResetPasswordDomainEvent(UserId userId)
        {
            UserId = userId;
        }

        public UserId UserId { get; }
    }
}