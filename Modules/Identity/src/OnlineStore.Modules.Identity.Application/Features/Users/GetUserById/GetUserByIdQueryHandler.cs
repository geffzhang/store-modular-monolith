using System.Threading.Tasks;
using AutoMapper;
using Common.Core.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Features.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Features.Users.GetUserById
{
    public class GetUserByIdQueryHandler:IQueryHandler<GetUserByIdQuery,UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository,IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<UserDto> HandleAsync(GetUserByIdQuery query)
        {
            var user = await _userRepository.FindByIdAsync(query.Id.ToString());

            return _mapper.Map<UserDto>(user);
        }
    }
}