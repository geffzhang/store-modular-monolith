﻿using System;

namespace Common.Domain.Types
{
    public class AggregateId<T> : IdentityBase<T>
    {
        public AggregateId(T id) : base(id)
        {
        }
        public static implicit operator T(AggregateId<T> id) => id.Value;
        public static implicit operator AggregateId<T>(T id) => new(id);
    }

    public class AggregateId : AggregateId<Guid>
    {
        public AggregateId() : this(Guid.NewGuid())
        {
        }

        public AggregateId(Guid value) : base(value)
        {
        }
    }
}