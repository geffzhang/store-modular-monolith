using System.Collections.Generic;
using BuildingBlocks.Core.Domain.Types;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Messaging.Events
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