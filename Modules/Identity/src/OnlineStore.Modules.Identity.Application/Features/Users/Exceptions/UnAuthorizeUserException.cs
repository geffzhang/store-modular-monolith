using Common.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Exceptions
{
    public class UnAuthorizeUserException:AppException
    {
        public UnAuthorizeUserException(string userName) : base($"UnAuthorize user exception for UserName : '{userName}'")
        {
        }
    }
}