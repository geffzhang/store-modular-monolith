using System.Collections.Generic;
using Common.Messaging.Events;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Events
{
    public class UserChangedEvent : GenericChangedEntryEvent<ApplicationUser>
    {
        public UserChangedEvent(IEnumerable<GenericChangedEntry<ApplicationUser>> changedEntries) : base(changedEntries)
        {
        }
    }
}
