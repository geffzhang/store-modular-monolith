using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Customers.Domain.Customers
{
    public class CustomerId : IdentityBase<Guid>
    {
        public CustomerId(Guid value) : base(value)
        {
        }
        
        public static implicit operator CustomerId(Guid id) => new(id);

        public static implicit operator Guid(CustomerId id) => id.Id;
    }
}