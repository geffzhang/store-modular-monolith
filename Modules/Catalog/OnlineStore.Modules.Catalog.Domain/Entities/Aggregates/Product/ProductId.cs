using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Catalog.Domain.Entities
{
    public class ProductId : IdentityBase<Guid>
    {
        private ProductId(Guid value) : base(value)
        {
        }

        public static implicit operator ProductId(Guid id)
        {
            return id == Guid.Empty ? null : new ProductId(id);
        }
    }
}