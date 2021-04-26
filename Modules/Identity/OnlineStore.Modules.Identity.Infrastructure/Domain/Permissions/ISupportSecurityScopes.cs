using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions
{
    public interface ISupportSecurityScopes
    {
        IEnumerable<string> Scopes { get; set; }
    }
}
