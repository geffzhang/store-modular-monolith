using Common.Domain;
using OnlineStore.Modules.Users.Domain.Aggregates.UserRegistrations;

namespace OnlineStore.Modules.Users.Domain.Rules
{
    public class UserRegistrationCannotBeExpiredMoreThanOnceRule : IBusinessRule
    {
        private readonly UserRegistrationStatus _actualRegistrationStatus;

        internal UserRegistrationCannotBeExpiredMoreThanOnceRule(UserRegistrationStatus actualRegistrationStatus)
        {
            _actualRegistrationStatus = actualRegistrationStatus;
        }

        public bool IsBroken()
        {
            return _actualRegistrationStatus == UserRegistrationStatus.Expired;
        }

        public string Message => "User Registration cannot be expired more than once";
    }
}