using System;
using System.Linq.Expressions;
using BuildingBlocks.Core.Persistence.Specification;

namespace OnlineStore.Modules.Identity.Domain.Aggregates.Users.Specs
{
    public class UserWithAccessTokenSpecification : SpecificationBase<User>
    {
        private readonly string _userName;
        public override Expression<Func<User, bool>> Criteria => user => user.UserName == _userName;

        public UserWithAccessTokenSpecification(string userName)
        {
            _userName = userName;
            AddInclude(u => u.RefreshTokens);
        }
    }
}