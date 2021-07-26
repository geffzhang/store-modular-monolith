using BuildingBlocks.Cqrs.Queries;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.Features.GetUserByLogin
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