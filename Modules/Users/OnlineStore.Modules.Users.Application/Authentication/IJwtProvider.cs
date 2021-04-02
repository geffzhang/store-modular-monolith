using System;
using System.Collections.Generic;

namespace OnlineStore.Modules.Users.Application.Authentication
{
    internal interface IJwtProvider
    {
        AuthDto Create(Guid userId, string username, string role, string audience = null,
            IDictionary<string, IEnumerable<string>> claims = null);
    }
}