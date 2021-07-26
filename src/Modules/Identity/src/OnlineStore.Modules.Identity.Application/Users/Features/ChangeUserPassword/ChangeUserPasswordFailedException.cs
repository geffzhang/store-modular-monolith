using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Features.ChangeUserPassword
{
    public class ChangeUserPasswordFailedException : AppException
    {
        public ChangeUserPasswordFailedException(string userId) : base($"An error occured while changing password for userId '{userId}'")
        {
        }
    }
}