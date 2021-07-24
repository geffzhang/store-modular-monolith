using Common.Authentication.Jwt;

namespace OnlineStore.Modules.Identity.Api.Authentications.Models.Response
{
    public class ExchangeRefreshTokenResponse
    {
        public JsonWebToken AccessToken { get; }
        public string RefreshToken { get; }

        public ExchangeRefreshTokenResponse(JsonWebToken accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}