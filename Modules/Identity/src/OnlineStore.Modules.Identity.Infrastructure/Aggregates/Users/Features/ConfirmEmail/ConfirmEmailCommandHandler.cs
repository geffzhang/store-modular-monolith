using System.Text;
using System.Threading.Tasks;
using Common.Core.Messaging.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using OnlineStore.Modules.Identity.Application.Users.ConfirmEmail;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Features.ConfirmEmail
{
    public class ConfirmEmailCommandHandler:ICommandHandler<ConfirmEmailCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task HandleAsync(ConfirmEmailCommand command)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.Code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded == false)
            {
                throw new ConfirmationEmailFailedException(command.UserId);
            }
        }
    }
}