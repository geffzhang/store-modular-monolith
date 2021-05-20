using System.Threading.Tasks;
using AutoMapper;
using Common.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Features.Users.GetUserByEmail
{
    public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByEmailQueryHandler(IUserRepository userRepository,IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> HandleAsync(GetUserByEmailQuery query)
        {
            var user = await _userRepository.FindByEmailAsync(query.Email);

            return _mapper.Map<UserDto>(user);
        }
    }
}