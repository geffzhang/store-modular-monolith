namespace OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses
{
    public class UserLockedResult
    {
        public bool Locked { get; set; }

        public UserLockedResult(bool locked)
        {
            Locked = locked;
        }
    }
}