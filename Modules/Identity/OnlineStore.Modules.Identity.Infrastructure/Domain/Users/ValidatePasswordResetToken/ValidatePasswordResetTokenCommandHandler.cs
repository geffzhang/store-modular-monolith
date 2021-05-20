using System.Threading.Tasks;
using Common.Messaging.Commands;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Features.Users.ValidatePasswordResetToken;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.ValidatePasswordResetToken
{
    public class ValidatePasswordResetTokenCommandHandler : ICommandHandler<ValidatePasswordResetTokenCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ValidatePasswordResetTokenCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task HandleAsync(ValidatePasswordResetTokenCommand command)
        {
            var applicationUser = await _userManager.FindByIdAsync(command.UserId);
            var tokenProvider = _userManager.Options.Tokens.PasswordResetTokenProvider;
            var result = await _userManager.VerifyUserTokenAsync(applicationUser, tokenProvider, "ResetPassword",
                command.Token);
        }
    }
}