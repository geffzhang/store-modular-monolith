using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Infrastructure.Events
{
    public class UserPasswordChangedEvent : DomainEventBase
    {
        public UserPasswordChangedEvent(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}
