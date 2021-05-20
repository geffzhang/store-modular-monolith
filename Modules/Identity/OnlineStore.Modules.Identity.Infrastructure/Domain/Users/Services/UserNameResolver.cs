using Microsoft.AspNetCore.Http;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Domain.Users.Services;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Services
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
            var identity = context?.User.Identity;
            if (identity is {IsAuthenticated: true})
            {
                result = context.Request.Headers["User-Name"];
                if (string.IsNullOrEmpty(result))
                {
                    result = identity.Name;
                }
            }
            return result;
        }
    }
}
