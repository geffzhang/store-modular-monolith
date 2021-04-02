namespace OnlineStore.Modules.Catalog.Domain.Entities
{
    public class ProductCategory
    {
        public bool IsFeaturedProduct { get; set; }

        public int DisplayOrder { get; set; }

        public CategoryId CategoryId { get; set; }

        public ProductId ProductId { get; set; }

        public Category Category { get; set; }

        public Product Product { get; set; }
    }
}