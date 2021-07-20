using System.Collections.Generic;
using Common.Core.Domain.Types;
using Newtonsoft.Json;

namespace Common.Core.Messaging.Events
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