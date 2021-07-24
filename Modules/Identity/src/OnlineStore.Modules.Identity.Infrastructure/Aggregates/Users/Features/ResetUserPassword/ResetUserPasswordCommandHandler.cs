using System.Threading.Tasks;
using Common.Core.Messaging.Commands;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Application.Users.ResetUserPassword;
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

        public async Task HandleAsync(ResetUserPasswordCommand command)
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
        }
    }
}