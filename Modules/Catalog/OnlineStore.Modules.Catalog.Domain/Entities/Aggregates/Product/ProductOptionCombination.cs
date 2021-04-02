using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Modules.Catalog.Domain.Entities
{
    public class ProductOptionCombination : EntityBase
    {
        public long ProductId { get; set; }

        public Product Product { get; set; }

        public long OptionId { get; set; }

        public ProductOption Option { get; set; }

        [StringLength(450)] public string Value { get; set; }

        public int SortIndex { get; set; }
    }
}