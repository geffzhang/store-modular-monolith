using Common.Core.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainEvents
{
    public class VerificationEmailSentEvent: DomainEventBase
    {
        public User User { get; }

        public VerificationEmailSentEvent(User user)
        {
            User = user;
        }
    }
}