using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users;
using OnlineStore.Modules.Identity.Application.Users.Dtos;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Mappings;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserResponse> AddAsync(User user)
        {
            var appUser = user.ToApplicationUser();
            IdentityResult identityResult;
            if (string.IsNullOrEmpty(user.Password))
                identityResult = await _userManager.CreateAsync(appUser);
            else
                identityResult = await _userManager.CreateAsync(appUser, appUser.Password);

            return new CreateUserResponse
            {
                Id = Guid.Parse(appUser.Id),
                Errors = identityResult.Succeeded ? null : identityResult.Errors.Select(e => e.Description),
                Success = identityResult.Succeeded
            };
        }

        public async Task<UserDto> FindByNameAsync(string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            return appUser.ToUserDto();
        }

        public async Task<UserDto> FindByIdAsync(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            return appUser.ToUserDto();
        }

        public async Task<UserDto> FindByEmailAsync(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            return appUser.ToUserDto();
        }

        public async Task<UserDto> FindByLoginAsync(string loginProvider, string providerKey)
        {
            var appUser = await _userManager.FindByLoginAsync(loginProvider, providerKey);
            return appUser.ToUserDto();
        }

        public async Task<bool> CheckPassword(User user, string password)
        {
            var appUser = user.ToApplicationUser();
            return await _userManager.CheckPasswordAsync(appUser, password);
        }
    }
}