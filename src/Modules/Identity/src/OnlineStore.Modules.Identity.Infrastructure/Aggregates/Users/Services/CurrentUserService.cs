using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OnlineStore.Modules.Identity.Application.Users.Contracts;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Services
{
    public class CurrentUserService: ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            UserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            IsAuthenticated = UserId != null;
        }

        public string UserId { get; }
        public bool IsAuthenticated { get; }
        public List<KeyValuePair<string, string>> Claims
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(
                    item.Type,
                    item.Value)).ToList();
            }
        }
    }
}