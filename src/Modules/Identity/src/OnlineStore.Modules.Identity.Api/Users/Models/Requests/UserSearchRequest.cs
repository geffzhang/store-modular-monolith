﻿using System;
using System.Collections.Generic;
using BuildingBlocks.Core.Types;

namespace OnlineStore.Modules.Identity.Api.Users.Models.Requests
{
    public class UserSearchRequest : SearchCriteriaBase
    {
        public string MemberId { get; set; }
        public IList<string> MemberIds { get; set; }
        public DateTime? ModifiedSinceDate { get; set; }
        //Search users by their role names
        public string[] Roles { get; set; }
    }
}