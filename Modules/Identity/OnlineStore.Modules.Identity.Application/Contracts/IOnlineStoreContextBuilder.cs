using Microsoft.AspNetCore.Http;
using OnlineStore.Modules.Identity.Domain.Common;

namespace OnlineStore.Modules.Identity.Application.Contracts
{
    public interface IOnlineStoreContextBuilder
    {
        HttpContext HttpContext { get; }
        StoreContext.OnlineStoreContext OnlineStoreContext { get; }
    }
}