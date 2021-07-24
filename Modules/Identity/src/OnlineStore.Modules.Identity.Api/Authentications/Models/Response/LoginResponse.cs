using Common.Authentication.Jwt;

namespace OnlineStore.Modules.Identity.Api.Authentications.Models.Response
{
    public class LoginResponse
    {
        public JsonWebToken AccessToken { get; }
        public string RefreshToken { get; }

        public LoginResponse(JsonWebToken accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}