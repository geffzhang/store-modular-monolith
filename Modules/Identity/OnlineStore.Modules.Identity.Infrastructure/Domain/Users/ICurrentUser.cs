namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users
{
    public interface ICurrentUser
    {
        string UserName { get; set; }
    }
}
