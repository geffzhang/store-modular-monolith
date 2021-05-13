using Microsoft.AspNetCore.Http;
using OnlineStore.Modules.Identity.Application.Users.Services;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users
{
    public class UserNameResolver : IUserNameResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserNameResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserName()
        {
            string result = "unknown";

            var context = _httpContextAccessor.HttpContext;
            if (context != null && context.Request != null && context.User != null)
            {
                var identity = context.User.Identity;
                if (identity != null && identity.IsAuthenticated)
                {
                    result = context.Request.Headers["User-Name"];
                    if (string.IsNullOrEmpty(result))
                    {
                        result = identity.Name;
                    }
                }
            }
            return result;

        }
    }
}
