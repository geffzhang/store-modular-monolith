namespace OnlineStore.Modules.Identity.Api.Authentications.Models
{
    public class ExchangeRefreshTokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}