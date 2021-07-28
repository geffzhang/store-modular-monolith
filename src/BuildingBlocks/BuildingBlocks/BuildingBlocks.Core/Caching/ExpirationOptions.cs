using System;

namespace BuildingBlocks.Core.Caching
{
    public class ExpirationOptions
    {
        public ExpirationOptions(DateTimeOffset absoluteExpiration)
        {
            AbsoluteExpiration = absoluteExpiration;
        }

        public DateTimeOffset AbsoluteExpiration { get; }
    }
}