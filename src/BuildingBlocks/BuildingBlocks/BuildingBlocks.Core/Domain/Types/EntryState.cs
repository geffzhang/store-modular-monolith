namespace BuildingBlocks.Core.Domain.Types
{
    public enum EntryState
    {
        Detached = 1,
        Unchanged = 2,
        Added = 4,
        Deleted = 8,
        Modified = 16,
    }
}
