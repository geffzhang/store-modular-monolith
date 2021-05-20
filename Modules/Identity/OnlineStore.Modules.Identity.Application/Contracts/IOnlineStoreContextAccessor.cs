namespace OnlineStore.Modules.Identity.Application.Context
{
    public interface IOnlineStoreContextAccessor
    {
        StoreContext.OnlineStoreContext OnlineStoreContext { get; set; }
    }
}