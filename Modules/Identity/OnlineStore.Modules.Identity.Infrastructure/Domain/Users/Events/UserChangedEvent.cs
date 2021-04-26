using System.Collections.Generic;
using Common.Messaging.Events;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserChangedEvent : GenericChangedEntryEvent<ApplicationUser>
    {
        public UserChangedEvent(IEnumerable<GenericChangedEntry<ApplicationUser>> changedEntries) : base(changedEntries)
        {
        }
    }
}
