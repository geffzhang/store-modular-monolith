using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Modules.Core.Domain.Domain.Entities
{
    public class WidgetZone : EntityBase
    {
        public WidgetZone(long id)
        {
            Id = id;
        }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}