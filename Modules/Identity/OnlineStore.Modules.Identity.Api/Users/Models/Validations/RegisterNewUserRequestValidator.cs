using FluentValidation;
using OnlineStore.Modules.Identity.Api.Users.Models.Requests;

namespace OnlineStore.Modules.Identity.Api.Users.Models.Validations
{
    public class RegisterNewUserRequestValidator : AbstractValidator<RegisterNewUserRequest>
    {
        public RegisterNewUserRequestValidator()
        {
            RuleFor(x => x.FirstName).Length(2, 30);
            RuleFor(x => x.LastName).Length(2, 30);
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.UserName).Length(3, 255);
            RuleFor(x => x.Password).Length(6, 15);
        }
    }
}