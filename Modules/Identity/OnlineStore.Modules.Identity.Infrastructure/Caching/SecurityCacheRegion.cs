using System;
using System.Collections.Concurrent;
using System.Threading;
using Common.Caching.Caching;
using Common.Identity;
using Microsoft.Extensions.Primitives;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Caching
{
    public class SecurityCacheRegion : CancellableCacheRegion<SecurityCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _usersRegionTokenLookup =
            new();

        public static IChangeToken CreateChangeTokenForUser(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var cancellationTokenSource = _usersRegionTokenLookup.GetOrAdd(user.Id, new CancellationTokenSource());
            return new CompositeChangeToken(new[]
            {
                CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token)
            });
        }

        public static void ExpireUser(ApplicationUser user)
        {
            if (_usersRegionTokenLookup.TryRemove(user.Id, out var token))
            {
                token.Cancel();
            }
        }
    }
}