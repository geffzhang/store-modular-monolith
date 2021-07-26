namespace OnlineStore.Modules.Identity.Api.Authentications.Models
{
    public class ExchangeRefreshTokenRequest
    {
        public ExchangeRefreshTokenRequest(string accessToken, string refreshToken, string signingKey)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            SigningKey = signingKey;
        }

        public string AccessToken { get;  }
        public string RefreshToken { get;  }
        public string SigningKey { get; }
    }
}