using System;
using System.Collections.Generic;

namespace BuildingBlocks.Core.Messaging
{
    public interface IContract
    {
        Type Type { get; }
        public IEnumerable<string> Required { get; }
    }
}