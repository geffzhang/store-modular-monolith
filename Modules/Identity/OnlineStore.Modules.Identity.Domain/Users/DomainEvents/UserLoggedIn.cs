using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainEvents
{
    public class UserLoginEvent : DomainEventBase
    {
        public UserLoginEvent(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}