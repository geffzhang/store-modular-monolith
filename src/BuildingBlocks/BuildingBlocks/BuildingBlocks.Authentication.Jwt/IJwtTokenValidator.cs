using System.Security.Claims;

namespace BuildingBlocks.Authentication.Jwt
{
    public interface IJwtTokenValidator
    {
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}