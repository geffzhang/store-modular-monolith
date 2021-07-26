using BuildingBlocks.Authentication.Jwt;
using Microsoft.Extensions.Logging;
using Moq;

namespace BuildingBlocks.Tests.Integration.Helpers
{
    public static class AuthHelper
    {
        private static readonly IJwtHandler JwtHandler;

        static AuthHelper()
        {
            var options = OptionsHelper.GetOptions<JwtOptions>("jwt");
            JwtHandler = new JwtHandler(options, null, Mock.Of<ILogger<JwtHandler>>());
        }

        public static string GenerateJwt(string userName, string email, string userId) =>
            JwtHandler.CreateToken(userName, email, userId).AccessToken;
    }
}