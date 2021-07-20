using System;
using System.Collections.Generic;

namespace Common.Core.Messaging
{
    public interface IContract
    {
        Type Type { get; }
        public IEnumerable<string> Required { get; }
    }
}