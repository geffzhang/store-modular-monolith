using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserLogoutEvent : DomainEventBase
    {
        public UserLogoutEvent(ApplicationUser user)
        {
            User = user;
        }

        public ApplicationUser User { get; set; }
    }
}
