using BuildingBlocks.Cqrs.Commands;

namespace OnlineStore.Modules.Identity.Application.Users.Features.SendVerificationEmail
{
    
    //https://github.com/kgrzybek/modular-monolith-with-ddd#38-internal-processing
    //https://github.com/kgrzybek/modular-monolith-with-ddd/discussions/185
    //https://github.com/kgrzybek/modular-monolith-with-ddd/discussions/119
    //https://github.com/kgrzybek/modular-monolith-with-ddd/discussions/187
    public class SendVerificationEmailCommand : InternalCommand
    {
        public SendVerificationEmailCommand(string userId, string? requestScheme, string? requestHost)
        {
            UserId = userId;
            RequestScheme = requestScheme;
            RequestHost = requestHost;
        }

        public string UserId { get; }
        public string RequestScheme { get; }
        public string RequestHost { get; }
    }
}