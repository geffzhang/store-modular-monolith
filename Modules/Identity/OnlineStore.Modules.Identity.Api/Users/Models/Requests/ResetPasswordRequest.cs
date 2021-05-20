namespace OnlineStore.Modules.Identity.Api.Users.Models.Requests
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public bool ForcePasswordChangeOnNextSignIn { get; set; }
    }
}