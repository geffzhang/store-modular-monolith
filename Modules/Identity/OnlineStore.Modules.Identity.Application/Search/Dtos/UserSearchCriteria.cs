using System;
using System.Collections.Generic;
using Common.Domain.Types;

namespace OnlineStore.Modules.Identity.Application.Search.Dtos
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