using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Authentication.Jwt
{
    public interface IJwtHandler
    {
        public JsonWebToken CreateToken(
            string userName,
            string email,
            string userId,
            bool? isVerified = null,
            string firstName = null,
            string lastName = null,
            string phoneNumber = null,
            IList<Claim> usersClaims = null,
            IList<string> rolesClaims = null,
            IList<Claim> permissionsClaims = null);

        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
        JsonWebTokenPayload GetTokenPayload(string accessToken);
        RefreshToken GenerateRefreshToken(string ipAddress, string userId);
    }
}