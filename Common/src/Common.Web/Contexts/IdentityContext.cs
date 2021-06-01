using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Common.Web.Contexts
{
    internal sealed class IdentityContext : IIdentityContext
    {
        internal IdentityContext()
        {
        }

        internal IdentityContext(ClaimsPrincipal principal)
        {
            Id = principal.Identity?.Name;
            IsAuthenticated = principal.Identity?.IsAuthenticated is true;
            IsAdmin = principal.IsInRole("admin");
            Claims = principal.Claims?.ToDictionary(x => x.Type, x => x.Value) ?? new Dictionary<string, string>();
        }

        internal static IIdentityContext Empty => new IdentityContext();
        public string Id { get; }
        public bool IsAuthenticated { get; }
        public bool IsAdmin { get; }
        public IDictionary<string, string> Claims { get; } = new Dictionary<string, string>();
    }
}