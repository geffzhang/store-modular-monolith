namespace OnlineStore.Modules.Identity.Application.Authentication.Dtos
{
    public class ResetPasswordConfirmRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public bool ForcePasswordChangeOnNextSignIn { get; set; }
    }
}