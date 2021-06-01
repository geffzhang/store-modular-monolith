using System.Collections.Generic;
using Common.Domain.Types;
using Common.Messaging.Events;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserChangedDomainEvent : DomainEventBase
    {
        public User User { get; }

        public UserChangedDomainEvent(User user)
        {
            User = user;
        }
    }
}