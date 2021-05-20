namespace OnlineStore.Modules.Identity.Domain.Users
{
    public interface IUserEditable
    {
        bool IsUserEditable(string userName);
    }
}