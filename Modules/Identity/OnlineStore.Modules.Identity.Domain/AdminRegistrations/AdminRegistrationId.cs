using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.AdminRegistrations
{
    public class AdminRegistrationId : IdentityBase<Guid>
    {
        public AdminRegistrationId(Guid value) : base(value)
        {
        }
        
        public static implicit operator AdminRegistrationId(Guid id) => new(id);

        public static implicit operator Guid(AdminRegistrationId id) => id.Id;
    }
}