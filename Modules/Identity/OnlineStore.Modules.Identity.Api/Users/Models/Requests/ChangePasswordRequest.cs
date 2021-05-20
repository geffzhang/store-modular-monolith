namespace OnlineStore.Modules.Identity.Api.Users.Models.Requests
{
    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}