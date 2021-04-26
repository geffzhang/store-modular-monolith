using Common.Domain;

namespace OnlineStore.Modules.Identity.Domain.UserRegistrations.Rules
{
    public class UserRegistrationCannotBeConfirmedAfterExpirationRule : IBusinessRule
    {
        private readonly UserRegistrationStatus _actualRegistrationStatus;

        internal UserRegistrationCannotBeConfirmedAfterExpirationRule(UserRegistrationStatus actualRegistrationStatus)
        {
            _actualRegistrationStatus = actualRegistrationStatus;
        }

        public bool IsBroken()
        {
            return _actualRegistrationStatus == UserRegistrationStatus.Expired;
        }

        public string Message => "User Registration cannot be confirmed because it is expired";
    }
}