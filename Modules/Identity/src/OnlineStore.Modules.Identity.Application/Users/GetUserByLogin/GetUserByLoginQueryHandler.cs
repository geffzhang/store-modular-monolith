using System.Threading.Tasks;
using AutoMapper;
using Common.Core.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.GetUserByLogin
{
    public class GetUserByLoginQueryHandler : IQueryHandler<GetUserByLoginQuery, UserDto>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public GetUserByLoginQueryHandler(IUserRepository repository,IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<UserDto> HandleAsync(GetUserByLoginQuery query)
        {
            var user = await _repository.FindByLoginAsync(query.LoginProvider, query.ProviderKey);

            return _mapper.Map<UserDto>(user);
        }
    }
}