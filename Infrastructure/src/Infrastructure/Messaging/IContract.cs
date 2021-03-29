using System;
using System.Collections.Generic;

namespace Infrastructure.Messaging
{
    public interface IContract
    {
        Type Type { get; }
        public IEnumerable<string> Required { get; }
    }
}