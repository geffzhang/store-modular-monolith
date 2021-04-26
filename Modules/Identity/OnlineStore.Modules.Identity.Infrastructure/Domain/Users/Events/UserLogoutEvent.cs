using Common.Domain.Types;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Events
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
