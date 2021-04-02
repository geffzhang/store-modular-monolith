namespace OnlineStore.Modules.Users.Application.Authentication
{
    internal interface IPasswordService
    {
        bool IsValid(string hash, string password);
        string Hash(string password);
    }
}