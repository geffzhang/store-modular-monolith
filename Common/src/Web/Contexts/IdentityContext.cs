using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Common.Web.Contexts
{
    public sealed class IdentityContext 
    {
        public IdentityContext(ClaimsPrincipal principal)
        {
            Id = principal.Identity?.Name;
            IsAuthenticated = principal.Identity?.IsAuthenticated is true;
            IsAdmin = principal.IsInRole("admin");
            Claims = principal.Claims.ToDictionary(x => x.Type, x => x.Value);
            Role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        public string Id { get; }
        public bool IsAuthenticated { get; }
        public bool IsAdmin { get; }
        public string Role { get; }
        public IDictionary<string, string> Claims { get; }
    }
}