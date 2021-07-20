using System.Threading.Tasks;
using AutoMapper;
using Common.Core.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Features.Users.GetUserByName
{
    public class GetUserByNameQueryHandler : IQueryHandler<GetUserByNameQuery, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public GetUserByNameQueryHandler(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<UserDto> HandleAsync(GetUserByNameQuery query)
        {
            var user = await _userRepository.FindByNameAsync(query.UserName);

            return _mapper.Map<UserDto>(user);
        }
    }
}