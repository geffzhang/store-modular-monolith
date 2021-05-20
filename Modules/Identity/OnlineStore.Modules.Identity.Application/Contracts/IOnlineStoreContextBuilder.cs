using Microsoft.AspNetCore.Http;

namespace OnlineStore.Modules.Identity.Application.Context
{
    public interface IOnlineStoreContextBuilder
    {
        HttpContext HttpContext { get; }
        StoreContext.OnlineStoreContext OnlineStoreContext { get; }
    }
}