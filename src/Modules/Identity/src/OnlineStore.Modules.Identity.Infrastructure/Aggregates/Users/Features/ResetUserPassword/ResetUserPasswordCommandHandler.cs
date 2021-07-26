using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Cqrs.Commands;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Application.Users.Features.ResetUserPassword;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Features.ResetUserPassword
{
    public class ResetUserPasswordCommandHandler : ICommandHandler<ResetUserPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetUserPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> HandleAsync(ResetUserPasswordCommand command,
            CancellationToken cancellationToken = default)
        {
            ApplicationUser appuser = null;
            if (string.IsNullOrEmpty(command.UserName) == false)
            {
                appuser = await _userManager.FindByNameAsync(command.UserName);
            }

            if (string.IsNullOrEmpty(command.UserId) == false)
            {
                appuser = await _userManager.FindByIdAsync(command.UserId);
            }

            if (appuser == null)
            {
                throw new UserNotFoundException(command.UserName);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(appuser);
            var result = await _userManager.ResetPasswordAsync(appuser, token, command.NewPassword);
            if (result.Succeeded)
            {
                appuser = await _userManager.FindByNameAsync(command.UserName);

                if (appuser.PasswordExpired != command.ForcePasswordChangeOnNextSignIn)
                {
                    appuser.PasswordExpired = command.ForcePasswordChangeOnNextSignIn;

                    await _userManager.UpdateAsync(appuser);
                }
            }
            else
            {
                throw new ResetPasswordFailedException(command.UserId);
            }

            return Unit.Result;
        }
    }
}