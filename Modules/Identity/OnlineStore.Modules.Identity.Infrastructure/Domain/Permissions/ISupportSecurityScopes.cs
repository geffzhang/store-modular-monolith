using System.Collections.Generic;

namespace Common.Identity
{
    public interface ISupportSecurityScopes
    {
        IEnumerable<string> Scopes { get; set; }
    }
}
