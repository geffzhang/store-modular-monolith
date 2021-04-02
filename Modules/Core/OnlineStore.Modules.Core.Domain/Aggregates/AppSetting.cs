using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Modules.Core.Domain.Domain.Entities
{
    public class AppSetting : EntityBaseWithTypedId<string>
    {
        public AppSetting(string id)
        {
            Id = id;
        }

        [StringLength(450)] public string Value { get; set; }

        [StringLength(450)] public string Module { get; set; }

        public bool IsVisibleInCommonSettingPage { get; set; }
    }
}