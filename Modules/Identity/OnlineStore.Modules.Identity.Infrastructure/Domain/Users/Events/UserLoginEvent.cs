using Common.Domain.Types;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Events
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
