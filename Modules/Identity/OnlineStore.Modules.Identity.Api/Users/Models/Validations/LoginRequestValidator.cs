using FluentValidation;
using OnlineStore.Modules.Identity.Api.Authentications.Models;

namespace OnlineStore.Modules.Identity.Api.Users.Models.Validations
{
    public class LoginRequestValidator  : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}