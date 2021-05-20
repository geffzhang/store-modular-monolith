namespace OnlineStore.Modules.Identity.Domain.Configurations.Settings
{
    public class Cors
    {
        public bool AllowAnyOrigin { get; set; }
        public string[] AllowedOrigins { get; set; }
    }
}