using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Modules.Catalog.Domain.Entities
{
    public class ProductTemplate : EntityBase
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        public string Name { get; set; }

        public IList<ProductTemplateProductAttribute> ProductAttributes { get; protected set; } =
            new List<ProductTemplateProductAttribute>();

        public void AddAttribute(long attributeId)
        {
            var productTempateProductAttribute = new ProductTemplateProductAttribute
            {
                ProductTemplate = this,
                ProductAttributeId = attributeId
            };
            ProductAttributes.Add(productTempateProductAttribute);
        }
    }
}