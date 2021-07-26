using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Authentication.Jwt
{
    public sealed class JwtHandler : IJwtHandler
    {
        private static readonly IDictionary<string, IEnumerable<string>> EmptyClaims =
            new Dictionary<string, IEnumerable<string>>();

        private static readonly ISet<string> DefaultClaims = new HashSet<string>
        {
            JwtRegisteredClaimNames.Sub,
            JwtRegisteredClaimNames.UniqueName,
            JwtRegisteredClaimNames.Jti,
            JwtRegisteredClaimNames.Iat,
            ClaimTypes.Role
        };

        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
        private readonly JwtOptions _options;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ILogger<JwtHandler> _logger;

        public JwtHandler(JwtOptions options,
            TokenValidationParameters tokenValidationParameters,
            ILogger<JwtHandler> logger)
        {
            _options = options;
            _tokenValidationParameters = tokenValidationParameters;
            _logger = logger;
        }

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
            IList<Claim> permissionsClaims = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User ID claim (subject) cannot be empty.", nameof(userName));

            var now = DateTime.UtcNow;
            string ipAddress = IpHelper.GetIpAddress();

            var jwtClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userName),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.UniqueName, userName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeMilliseconds().ToString()),
                new(ClaimTypes.NameIdentifier, userId),
                new(ClaimTypes.Name, firstName ?? string.Empty),
                new(ClaimTypes.Surname, lastName ?? string.Empty),
                new(ClaimTypes.MobilePhone, phoneNumber ?? string.Empty),
                new("ip", ipAddress)
            };

            if (rolesClaims?.Any() is true)
            {
                foreach (var role in rolesClaims)
                {
                    jwtClaims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            if (permissionsClaims?.Any() is true)
            {
                jwtClaims = jwtClaims.Union(permissionsClaims).ToList();
            }

            if (usersClaims?.Any() is true)
            {
                jwtClaims = jwtClaims.Union(usersClaims).ToList();
            }

            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.IssuerSigningKey));
            if (issuerSigningKey is null) throw new InvalidOperationException("Issuer signing key not set.");
            var signingCredentials =
                new SigningCredentials(issuerSigningKey, _options.Algorithm ?? SecurityAlgorithms.HmacSha256);

            var expire = now.AddMinutes(_options.ExpiryMinutes);

            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                notBefore: now,
                claims: jwtClaims,
                expires: expire,
                signingCredentials: signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            var dicPermissionClaim = permissionsClaims?.GroupBy(claim => claim.Type)
                .ToDictionary(group => group.Key, group => group.Select(x => x.Value).ToList() as IList<string>);

            var refreshToken = GenerateRefreshToken(userId);

            return new JsonWebToken
            {
                IsVerified = isVerified,
                AccessToken = token,
                RefreshToken = refreshToken,
                Expires = expire.ToUnixTimeMilliseconds(),
                UserId = userId,
                Email = email,
                Roles = rolesClaims?.ToList() ?? Enumerable.Empty<string>().ToList(),
                Permissions = dicPermissionClaim,
            };
        }

        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            try
            {
                var principal =
                    _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals
                    (_options.Algorithm ?? SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception e)
            {
                _logger.LogError($"Token validation failed: {e.Message}");
                return null;
            }
        }

        public RefreshToken GenerateRefreshToken(string userId, string ip = null)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(_options.RefreshTokenExpiryInDays),
                Created = DateTime.UtcNow,
                IpAddress = ip ?? IpHelper.GetIpAddress(),
            };
        }


        public JsonWebTokenPayload GetTokenPayload(string accessToken)
        {
            _jwtSecurityTokenHandler.ValidateToken(accessToken, _tokenValidationParameters,
                out var validatedSecurityToken);
            if (!(validatedSecurityToken is JwtSecurityToken jwt)) return null;

            return new JsonWebTokenPayload
            {
                Subject = jwt.Subject,
                Role = jwt.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
                Expires = jwt.ValidTo.ToUnixTimeMilliseconds(),
                Claims = jwt.Claims.Where(x => !DefaultClaims.Contains(x.Type))
                    .GroupBy(c => c.Type)
                    .ToDictionary(k => k.Key, v => v.Select(c => c.Value))
            };
        }
    }
}