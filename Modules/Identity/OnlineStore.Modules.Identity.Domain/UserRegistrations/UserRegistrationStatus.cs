using System.Collections.Generic;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.UserRegistrations
{
    public class UserRegistrationStatus : ValueObject
    {
        private UserRegistrationStatus(string value)
        {
            Value = value;
        }

        public static UserRegistrationStatus WaitingForConfirmation =>
            new(nameof(WaitingForConfirmation));

        public static UserRegistrationStatus Confirmed => new(nameof(Confirmed));

        public static UserRegistrationStatus Expired => new(nameof(Expired));

        public string Value { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}