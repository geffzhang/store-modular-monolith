namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users
{
    public interface IUserNameResolver
    {
        string GetCurrentUserName();
    }
}
