using System.Collections.Generic;

namespace OnlineStore.Modules.Identity.Application.Users.Services
{
    public interface ICurrentUserService
    {
        public string UserId { get; }

        public bool IsAuthenticated { get; }

        public List<KeyValuePair<string, string>> Claims { get; }
    }
}
