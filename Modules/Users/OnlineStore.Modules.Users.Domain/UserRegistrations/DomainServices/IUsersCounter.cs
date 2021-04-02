namespace OnlineStore.Modules.Users.Domain.Aggregates.UserRegistrations.DomainServices
{
    public interface IUsersCounter
    {
        int CountUsersWithLogin(string login);
    }
}