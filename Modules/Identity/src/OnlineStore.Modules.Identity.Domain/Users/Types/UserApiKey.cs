namespace OnlineStore.Modules.Identity.Domain.Users.Types
{
    public class UserApiKey 
    {
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
