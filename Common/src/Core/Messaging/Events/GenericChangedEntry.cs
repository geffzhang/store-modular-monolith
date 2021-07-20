using Common.Core.Domain.Types;
using Newtonsoft.Json;

namespace Common.Core.Messaging.Events
{
    public class GenericChangedEntry<T> 
    {
        public GenericChangedEntry(T entry, EntryState state)
            : this(entry, entry, state)
        {
        }

        [JsonConstructor]
        public GenericChangedEntry(T newEntry, T oldEntry, EntryState entryState)
        {
            NewEntry = newEntry;
            OldEntry = oldEntry;
            EntryState = entryState;
        }

        public EntryState EntryState { get; set; }
        public T NewEntry { get; set; }
        public T OldEntry { get; set; }
    }
}