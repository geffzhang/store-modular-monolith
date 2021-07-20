using Common.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Exceptions
{
    public class CurrentUserIsEmptyException:AppException
    {
        public CurrentUserIsEmptyException() : base("Current user can't be null.")
        {
        }
    }
}