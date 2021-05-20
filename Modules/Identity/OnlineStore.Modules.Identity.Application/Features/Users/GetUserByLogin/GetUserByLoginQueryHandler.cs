using System.Threading.Tasks;
using Common.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.GetUserByLogin
{
    public class GetUserByLoginQueryHandler : IQueryHandler<GetUserByLoginQuery, UserDto>
    {
        private readonly IUserRepository _repository;

        public GetUserByLoginQueryHandler(IUserRepository repository)
        {
            _repository = repository;
        }
        public Task<UserDto> HandleAsync(GetUserByLoginQuery query)
        {
            return _repository.FindByLoginAsync(query.LoginProvider, query.ProviderKey);
        }
    }
}