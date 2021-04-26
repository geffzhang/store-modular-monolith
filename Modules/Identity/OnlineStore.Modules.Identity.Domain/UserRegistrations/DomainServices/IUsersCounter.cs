namespace OnlineStore.Modules.Users.Domain.UserRegistrations.DomainServices
{
    public interface IUsersCounter
    {
        int CountUsersWithLogin(string login);
    }
}