using System.Collections.Generic;
using Common.Authentication.Jwt;

namespace Common.Tests.Integration.Helpers
{
    public static class AuthHelper
    {
        private static readonly IJwtHandler JwtHandler;

        static AuthHelper()
        {
            var options = OptionsHelper.GetOptions<JwtOptions>("jwt");
            JwtHandler = new JwtHandler(options,null);
        }

        public static string GenerateJwt(string userId, string role = null, string audience = null,
            IDictionary<string, IEnumerable<string>> claims = null)
            => JwtHandler.CreateToken(userId, role, audience, claims).AccessToken;
    }
}
