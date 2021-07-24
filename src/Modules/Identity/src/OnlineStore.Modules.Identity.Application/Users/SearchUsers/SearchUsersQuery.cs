using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BuildingBlocks.Core.Messaging.Queries;
using BuildingBlocks.Core.Types;

namespace OnlineStore.Modules.Identity.Application.Users.SearchUsers
{
    public class SearchUsersQuery : SearchCriteriaBase, IQuery<UserSearchResponse>
    {
        public SearchUsersQuery(string memberId = null,
            IReadOnlyList<string> memberIds = null,
            DateTime? modifiedSinceDate = null,
            IReadOnlyList<string> roles = null)
        {
            MemberId = memberId;
            MemberIds = memberIds?.ToImmutableList();
            ModifiedSinceDate = modifiedSinceDate;
            Roles = roles;
        }

        public string MemberId { get; }
        public IReadOnlyList<string> MemberIds { get; }
        public DateTime? ModifiedSinceDate { get; }
        public IReadOnlyList<string> Roles { get; }
    }
}