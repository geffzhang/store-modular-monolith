using System;
using BuildingBlocks.Cqrs.Commands;

namespace OnlineStore.Modules.Identity.Application.Users.Features.ForgotPassword
{
    public class ForgetPasswordCommand : ICommand
    {
        public ForgetPasswordCommand(string email,string requestHost,string requestScheme)
        {
            Email = email;
            RequestHost = requestHost;
            RequestScheme = requestScheme;
        }

        public string Email { get; }
        public string RequestScheme { get; }
        public string RequestHost { get; }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}