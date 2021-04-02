using Common.Domain;

namespace OnlineStore.Modules.Users.Domain.UserRegistrations.Rules
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