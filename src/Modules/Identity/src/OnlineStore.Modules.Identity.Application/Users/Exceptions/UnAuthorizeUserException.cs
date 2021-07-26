using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
{
    public class UnAuthorizeUserException:AppException
    {
        public string UserName { get; }
        public UnAuthorizeUserException(string userName) : base($"UnAuthorize user exception for UserName : '{userName}'")
        {
            userName = userName;
        }
    }
}