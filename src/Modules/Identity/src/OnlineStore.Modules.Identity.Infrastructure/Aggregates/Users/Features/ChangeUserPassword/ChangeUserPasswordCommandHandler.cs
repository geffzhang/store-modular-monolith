using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Cqrs.Commands;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Application.Users.Features.ChangeUserPassword;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Features.ChangeUserPassword
{
    public class ChangeCurrentUserPasswordCommandHandler : ICommandHandler<ChangeUserPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangeCurrentUserPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> HandleAsync(ChangeUserPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            var appuser = await _userManager.FindByNameAsync(command.UserName);
            if (appuser == null)
            {
                throw new UserNotFoundException(command.UserName);
            }

            if (command.OldPassword == command.NewPassword)
            {
                throw new PasswordShouldNotEqualOldException();
            }

            var result = await _userManager.ChangePasswordAsync(appuser, command.OldPassword,
                command.NewPassword);
            if (result.Succeeded && appuser.PasswordExpired)
            {
                appuser.PasswordExpired = false;
                await _userManager.UpdateAsync(appuser);
            }
            else
            {
                throw new ChangeUserPasswordFailedException(command.UserName);
            }

            return Unit.Result;
        }
    }
}