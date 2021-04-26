namespace OnlineStore.Modules.Identity.Domain.UserRegistrations.DomainServices
{
    public interface IUsersCounter
    {
        int CountUsersWithLogin(string login);
    }
}