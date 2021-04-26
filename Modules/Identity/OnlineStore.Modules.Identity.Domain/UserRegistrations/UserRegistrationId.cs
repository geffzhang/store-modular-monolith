using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.UserRegistrations
{
    public class UserRegistrationId : IdentityBase<Guid>
    {
        public UserRegistrationId(Guid value) : base(value)
        {
        }
        
        public static implicit operator UserRegistrationId(Guid id) => new(id);

        public static implicit operator Guid(UserRegistrationId id) => id.Id;
    }
}