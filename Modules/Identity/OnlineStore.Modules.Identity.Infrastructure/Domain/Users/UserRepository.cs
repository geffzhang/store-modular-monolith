using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Application.Users.Dtos;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly SecurityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserRepository(SecurityDbContext userAccessContext, UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _context = userAccessContext;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<CreateUserResponse> AddAsync(User user)
        {
            var appUser = _mapper.Map<ApplicationUser>(user);
            var identityResult = await _userManager.CreateAsync(appUser, appUser.Password);
            return new CreateUserResponse(Guid.Parse(appUser.Id), identityResult.Succeeded,
                identityResult.Succeeded ? null : identityResult.Errors.Select(e => e.Description));
        }
        
        public async Task<User> FindByName(string userName)
        {
            return _mapper.Map<User>(await _userManager.FindByNameAsync(userName));
        }

        public async Task<bool> CheckPassword(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(_mapper.Map<ApplicationUser>(user), password);
        }
    }
}