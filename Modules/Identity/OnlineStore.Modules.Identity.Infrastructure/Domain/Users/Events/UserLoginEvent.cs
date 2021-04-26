using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserLoginEvent : DomainEventBase
    {
        public UserLoginEvent(ApplicationUser user)
        {
            User = user;
        }

        public ApplicationUser User { get; set; }
    }
}
