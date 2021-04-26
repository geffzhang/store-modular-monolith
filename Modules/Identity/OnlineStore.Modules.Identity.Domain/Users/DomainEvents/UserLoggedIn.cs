using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users.DomainEvents
{
    public class UserLoggedIn : DomainEventBase
    {
        public UserLoggedIn(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; set; }
    }
}