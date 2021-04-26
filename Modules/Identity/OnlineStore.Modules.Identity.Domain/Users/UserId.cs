using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Users
{
    public class UserId : IdentityBase<Guid>
    {
        public UserId(Guid value) : base(value)
        {
        }
        
        public static implicit operator UserId(Guid id) => new(id);

        public static implicit operator Guid(UserId userId) => userId.Id;
    }
}