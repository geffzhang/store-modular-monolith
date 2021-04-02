using System.Collections.Generic;

namespace OnlineStore.Modules.Catalog.Domain.Entities
{
    public class ProductAttributeGroup
    {
        public EntityId Id { get; set; }
        public string Name { get; set; }

        public IList<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
    }
}