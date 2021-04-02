using System.Collections.Generic;

namespace OnlineStore.Modules.Catalog.Domain.Entities
{
    public class ProductAttribute
    {
        public EntityId Id { get; set; }
        public string Name { get; set; }

        public EntityId GroupId { get; set; }

        public ProductAttributeGroup Group { get; set; }

        public IList<ProductTemplateProductAttribute> ProductTemplates { get; protected set; } =
            new List<ProductTemplateProductAttribute>();
    }
}