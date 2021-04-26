using Common.Domain.Types;

namespace Common.Identity
{
    public class ApplicationUserLogin 
    {
        public virtual string LoginProvider { get; set; }
        public virtual string ProviderKey { get; set; }
    }
}
