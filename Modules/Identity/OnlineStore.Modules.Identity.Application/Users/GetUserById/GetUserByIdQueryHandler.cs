using System.Threading.Tasks;
using AutoMapper;
using Common.Messaging.Queries;
using OnlineStore.Modules.Identity.Application.Users.Dtos;

namespace OnlineStore.Modules.Identity.Application.Users.GetUserById
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

            return user;
        }
    }
}