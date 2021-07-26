using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BuildingBlocks.Cqrs.Queries;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.Features.GetUserByLogin
{
    public class GetUserByLoginQueryHandler : IQueryHandler<GetUserByLoginQuery, UserDto>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public GetUserByLoginQueryHandler(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserDto> HandleAsync(GetUserByLoginQuery query, CancellationToken cancellationToken = default)
        {
            var user = await _repository.FindByLoginAsync(query.LoginProvider, query.ProviderKey);

            return _mapper.Map<UserDto>(user);
        }
    }
}