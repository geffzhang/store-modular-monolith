using System.Text;
using System.Threading.Tasks;
using Common.Mail;
using Common.Messaging.Commands;
using Common.Scheduling;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using OnlineStore.Modules.Identity.Application.Features.Users.SendVerificationEmail;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.SendVerificationEmail
{
    public class SendVerificationEmailCommandHandler : ICommandHandler<SendVerificationEmailCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessagesScheduler _messagesScheduler;
        private readonly IMailService _mailService;

        public SendVerificationEmailCommandHandler(UserManager<ApplicationUser> userManager,
            IMessagesScheduler messagesScheduler, IMailService mailService)
        {
            _userManager = userManager;
            _messagesScheduler = messagesScheduler;
            _mailService = mailService;
        }

        public async Task HandleAsync(SendVerificationEmailCommand command)
        {
            var applicationUser = (await _userManager.FindByIdAsync(command.UserId));
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl =
                $"{command.RequestScheme}://{command.RequestHost}/#/confirm-email/{applicationUser.Id}/{encodedCode}";
            
            string link = $"<a href='{callbackUrl}'>link</a>";
            string content = $"Welcome to Online Shopping application! Please confirm your registration using this {link}.";
            
            // Send Email
            await _mailService.SendAsync(new MailRequest(applicationUser.Email, "Confirmation Email", content));
        }
    }
}