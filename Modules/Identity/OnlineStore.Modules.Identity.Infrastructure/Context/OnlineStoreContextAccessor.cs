using Microsoft.AspNetCore.Http;
using OnlineStore.Modules.Identity.Application.Contracts;
using OnlineStore.Modules.Identity.Domain.Common;

namespace OnlineStore.Modules.Identity.Infrastructure.Context
{
    public class OnlineStoreContextAccessor : IOnlineStoreContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OnlineStoreContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public StoreContext.OnlineStoreContext OnlineStoreContext
        {
            get => _httpContextAccessor.HttpContext?.Items["OnlineStoreContext"] as StoreContext.OnlineStoreContext;
            set
            {
                if (_httpContextAccessor?.HttpContext != null)
                    _httpContextAccessor.HttpContext.Items["OnlineStoreContext"] = value;
            }
        }
    }
}