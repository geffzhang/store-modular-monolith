namespace OnlineStore.Modules.Identity.Domain.Users
{
    public interface IUserNameResolver
    {
        string GetCurrentUserName();
    }
}
