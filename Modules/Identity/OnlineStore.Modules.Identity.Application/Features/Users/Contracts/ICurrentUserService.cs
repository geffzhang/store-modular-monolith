﻿using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Contracts
{
    public interface ICurrentUserService
    {
        public string UserId { get; }

        public bool IsAuthenticated { get; }

        public List<KeyValuePair<string, string>> Claims { get; }
    }
}
