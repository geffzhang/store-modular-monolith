namespace OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses
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