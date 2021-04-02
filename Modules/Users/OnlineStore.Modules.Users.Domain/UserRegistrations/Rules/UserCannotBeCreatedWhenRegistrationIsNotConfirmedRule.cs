using Common.Domain;
using OnlineStore.Modules.Users.Domain.Aggregates.UserRegistrations;

namespace OnlineStore.Modules.Users.Domain.Rules
{
    public class UserCannotBeCreatedWhenRegistrationIsNotConfirmedRule : IBusinessRule
    {
        private readonly UserRegistrationStatus _actualRegistrationStatus;

        internal UserCannotBeCreatedWhenRegistrationIsNotConfirmedRule(UserRegistrationStatus actualRegistrationStatus)
        {
            _actualRegistrationStatus = actualRegistrationStatus;
        }

        public bool IsBroken()
        {
            return _actualRegistrationStatus != UserRegistrationStatus.Confirmed;
        }

        public string Message => "User cannot be created when registration is not confirmed";
    }
}