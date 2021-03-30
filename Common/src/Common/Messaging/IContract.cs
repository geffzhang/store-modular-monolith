using System;
using System.Collections.Generic;

namespace Common.Messaging
{
    public interface IContract
    {
        Type Type { get; }
        public IEnumerable<string> Required { get; }
    }
}