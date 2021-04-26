using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainEvents
{
    public class UserLoggedOut: DomainEventBase
    {
        public UserLoggedOut(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}