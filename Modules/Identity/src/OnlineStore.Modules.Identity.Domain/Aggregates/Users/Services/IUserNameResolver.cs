namespace OnlineStore.Modules.Identity.Domain.Aggregates.Users.Services
{
    public interface IUserNameResolver
    {
        string GetCurrentUserName();
    }
}
