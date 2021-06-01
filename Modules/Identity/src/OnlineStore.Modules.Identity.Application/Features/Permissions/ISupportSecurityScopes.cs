using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Application.Features.Permissions
{
    public interface ISupportSecurityScopes
    {
        IEnumerable<string> Scopes { get; set; }
    }
}
