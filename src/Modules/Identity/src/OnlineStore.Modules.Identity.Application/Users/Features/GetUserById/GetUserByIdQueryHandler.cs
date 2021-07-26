using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BuildingBlocks.Cqrs.Queries;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.Features.GetUserById
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByIdAsync(query.Id.ToString());

            return _mapper.Map<UserDto>(user);
        }
    }
}