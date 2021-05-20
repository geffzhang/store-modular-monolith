using OnlineStore.Modules.Identity.Domain.Common;

namespace OnlineStore.Modules.Identity.Application.Contracts
{
    public interface IOnlineStoreContextAccessor
    {
        StoreContext.OnlineStoreContext OnlineStoreContext { get; set; }
    }
}