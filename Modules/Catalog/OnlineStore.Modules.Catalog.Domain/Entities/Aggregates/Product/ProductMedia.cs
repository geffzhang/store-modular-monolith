namespace OnlineStore.Modules.Catalog.Domain.Entities
{
    public class ProductMedia : EntityBase
    {
        public long ProductId { get; set; }

        public Product Product { get; set; }

        public long MediaId { get; set; }

        public Media Media { get; set; }

        public int DisplayOrder { get; set; }
    }
}