using Common.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Users.Dtos;

namespace OnlineStore.Modules.Identity.Application.Users.GetUserByLogin
{
    public class GetUserByLoginQuery : IQuery<UserDto>
    {
        public GetUserByLoginQuery(string loginProvider,string providerKey)
        {
            ProviderKey = providerKey;
            LoginProvider = loginProvider;
        }
        public string ProviderKey { get; }
        public string LoginProvider { get;  }
    }
}