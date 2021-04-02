using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Catalog.Domain.Entities
{
    public class CategoryId : IdentityBase<Guid>
    {
        private CategoryId(Guid value) : base(value)
        {
        }

        public static implicit operator CategoryId(Guid id)
        {
            return id == Guid.Empty ? null : new CategoryId(id);
        }
    }
}