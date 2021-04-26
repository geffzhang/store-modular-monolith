using System.Collections.Generic;
using Common.Messaging.Events;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events
{
    public class UserChangingEvent : GenericChangedEntryEvent<ApplicationUser>
    {
        public UserChangingEvent(IEnumerable<GenericChangedEntry<ApplicationUser>> changedEntries) : base(changedEntries)
        {
        }
    }
}
