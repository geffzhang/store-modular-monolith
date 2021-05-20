namespace OnlineStore.Modules.Identity.Domain.Users.Services
{
    public interface IUserEditable
    {
        bool IsUserEditable(string userName);
    }
}