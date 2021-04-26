namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users
{
    public class ApplicationUserLogin 
    {
        public virtual string LoginProvider { get; set; }
        public virtual string ProviderKey { get; set; }
    }
}
