using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Application.Permissions
{
    public interface ISupportSecurityScopes
    {
        IEnumerable<string> Scopes { get; set; }
    }
}
