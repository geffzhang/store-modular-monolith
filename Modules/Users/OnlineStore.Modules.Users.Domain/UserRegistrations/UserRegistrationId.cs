using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Users.Domain.Aggregates.UserRegistrations
{
    public class UserRegistrationId : IdentityBase<Guid>
    {
        public UserRegistrationId(Guid value) : base(value)
        {
        }
    }
}