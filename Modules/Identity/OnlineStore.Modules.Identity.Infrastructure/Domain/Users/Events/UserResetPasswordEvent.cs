using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserResetPasswordEvent : DomainEventBase
    {
        public UserResetPasswordEvent(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}
