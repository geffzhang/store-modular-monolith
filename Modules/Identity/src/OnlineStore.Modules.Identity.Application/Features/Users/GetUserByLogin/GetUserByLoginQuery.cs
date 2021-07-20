using Common.Core.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Features.Users.GetUserByLogin
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