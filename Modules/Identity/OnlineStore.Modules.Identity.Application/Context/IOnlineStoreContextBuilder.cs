namespace OnlineStore.Modules.Identity.Infrastructure.Context
{
    public interface IOnlineStoreContextBuilder
    {
        HttpContext HttpContext { get; }
        IOnlineStoreContext OnlineStoreContext { get; }
    }
}