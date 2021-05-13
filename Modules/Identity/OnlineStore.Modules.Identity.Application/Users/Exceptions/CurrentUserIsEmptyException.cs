using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
{
    public class CurrentUserIsEmptyException:AppException
    {
        public CurrentUserIsEmptyException() : base("Current user can't be null.")
        {
        }
    }
}