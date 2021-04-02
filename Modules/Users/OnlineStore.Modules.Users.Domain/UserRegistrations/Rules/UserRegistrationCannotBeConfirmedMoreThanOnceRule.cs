using Common.Domain;
using OnlineStore.Modules.Users.Domain.Aggregates.UserRegistrations;

namespace OnlineStore.Modules.Users.Domain.Rules
{
    public class UserRegistrationCannotBeConfirmedMoreThanOnceRule : IBusinessRule
    {
        private readonly UserRegistrationStatus _actualRegistrationStatus;

        internal UserRegistrationCannotBeConfirmedMoreThanOnceRule(UserRegistrationStatus actualRegistrationStatus)
        {
            _actualRegistrationStatus = actualRegistrationStatus;
        }

        public bool IsBroken()
        {
            return _actualRegistrationStatus == UserRegistrationStatus.Confirmed;
        }

        public string Message => "User Registration cannot be confirmed more than once";
    }
}