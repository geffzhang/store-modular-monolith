using System;
using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Domain.Search
{
    public class UserSearchCriteria : SearchCriteriaBase
    {
        public string MemberId { get; set; }
        public IList<string> MemberIds { get; set; }
        public DateTime? ModifiedSinceDate { get; set; }
        //Search users by their role names
        public string[] Roles { get; set; }
    }
}