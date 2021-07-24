using System.Threading.Tasks;
using Common.Core.Messaging.Commands;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users.ValidatePasswordResetToken;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Features.ValidatePasswordResetToken
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