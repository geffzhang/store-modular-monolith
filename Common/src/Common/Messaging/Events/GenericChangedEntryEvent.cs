using System.Collections.Generic;
using Common.Domain.Types;
using Newtonsoft.Json;

namespace Common.Messaging.Events
{
    public class GenericChangedEntryEvent<T> : DomainEventBase
    {
        [JsonConstructor]
        public GenericChangedEntryEvent(IEnumerable<GenericChangedEntry<T>> changedEntries)
        {
            ChangedEntries = changedEntries;
        }

        public IEnumerable<GenericChangedEntry<T>> ChangedEntries { get; private set; }
    }
}