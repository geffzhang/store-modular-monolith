using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BuildingBlocks.Cqrs.Queries;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.Features.GetUserByEmail
{
    public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByEmailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> HandleAsync(GetUserByEmailQuery query, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByEmailAsync(query.Email);

            return _mapper.Map<UserDto>(user);
        }
    }
}