using System;
using Common.Domain.Types;

namespace OnlineStore.Modules.Users.Domain.Aggregates.Users
{
    public class UserId : IdentityBase<Guid>
    {
        public UserId(Guid value) : base(value)
        {
        }
    }
}