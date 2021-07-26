using System;
using BuildingBlocks.Cqrs.Commands;

namespace OnlineStore.Modules.Identity.Application.Users.Features.RequestPasswordReset
{
    public class RequestPasswordResetCommand : ICommand
    {
        public RequestPasswordResetCommand(string loginOrEmail, string requestScheme, string requestHost)
        {
            LoginOrEmail = loginOrEmail;
            RequestHost = requestHost;
            RequestScheme = requestScheme;
        }

        public string LoginOrEmail { get; }
        public string RequestScheme { get; }
        public string RequestHost { get; }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}