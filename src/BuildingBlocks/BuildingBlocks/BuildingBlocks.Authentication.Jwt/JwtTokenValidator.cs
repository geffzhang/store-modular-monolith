using System;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Authentication.Jwt
{
    public class JwtTokenValidator : IJwtTokenValidator
    {
        private readonly IJwtHandler _jwtTokenHandler;
        private readonly JwtOptions _options;

        internal JwtTokenValidator(IJwtHandler jwtTokenHandler, JwtOptions options)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _options = options;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.IssuerSigningKey));
            if (issuerSigningKey is null) throw new InvalidOperationException("Issuer signing key not set.");

            return _jwtTokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = issuerSigningKey,
                ValidateLifetime = false ,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero,
            });
        }
    }
}