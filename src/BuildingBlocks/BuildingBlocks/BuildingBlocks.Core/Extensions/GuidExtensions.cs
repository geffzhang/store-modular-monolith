using System;

namespace BuildingBlocks.Core.Extensions
{
    public static class GuidExtensions
    {
        public static Guid BindId(this Guid id)
        {
            return id == Guid.Empty ? Guid.NewGuid() : id;
        }
    }
}