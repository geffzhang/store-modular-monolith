using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
{
    public class UnAuthorizeUserException:AppException
    {
        public UnAuthorizeUserException(string userName) : base($"UnAuthorize user exception for UserName : '{userName}'")
        {
        }
    }
}