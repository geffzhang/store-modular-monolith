namespace OnlineStore.Modules.Identity.Domain.Settings
{
    public class Cors
    {
        public bool AllowAnyOrigin { get; set; }
        public string[] AllowedOrigins { get; set; }
    }
}