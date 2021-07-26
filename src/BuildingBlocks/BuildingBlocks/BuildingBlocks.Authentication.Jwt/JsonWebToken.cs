using System.Collections.Generic;

namespace BuildingBlocks.Authentication.Jwt
{
    public class JsonWebToken
    {
        public string AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public long Expires { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool? IsVerified { get; set; }
        public IList<string> Roles { get; set; }
        public Dictionary<string, IList<string>> Permissions { get; set; }
    }
}