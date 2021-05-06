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
        private readonly IdentityDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(IdentityDbContext userAccessContext, UserManager<ApplicationUser> userManager)
        {
            _context = userAccessContext;
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
            return new CreateUserResponse(Guid.Parse(appUser.Id), identityResult.Succeeded,
                identityResult.Succeeded ? null : identityResult.Errors.Select(e => e.Description));
        }

        public async Task<User> FindByNameAsync(string userName)
        {
           var appUser = await _userManager.FindByNameAsync(userName);
           return appUser.ToUser();
        }

        public async Task<User> FindByIdAsync(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            return appUser.ToUser();
        }

        public async Task<bool> CheckPassword(User user, string password)
        {
            var appUser = user.ToApplicationUser();
            return await _userManager.CheckPasswordAsync(appUser, password);
        }
    }
}