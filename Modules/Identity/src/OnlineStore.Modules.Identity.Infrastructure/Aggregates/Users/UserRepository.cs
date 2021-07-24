using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Core.Types;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users.Contracts;
using OnlineStore.Modules.Identity.Application.Users.Dtos.GatewayResponses.Repositories;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users
{
	//Aggregate (Root) Design: Separate Behavior & Data for Persistence: https://www.youtube.com/watch?v=GtWVGJp061A
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
                identityResult = await _userManager.CreateAsync(appUser, user.Password);

            return new CreateUserResponse(Guid.Parse(appUser.Id), identityResult.Succeeded,
                identityResult.Errors.Select(e => new Error(e.Code, e.Description)));
        }

        public async Task<UpdateUserResponse> UpdateAsync(User user)
        {
            var appUser = user.ToApplicationUser();
            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            return new UpdateUserResponse(appUser.ToUser(), identityResult.Succeeded,
                identityResult.Errors.Select(e => new Error(e.Code, e.Description)));
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

        public async Task<User> FindByEmailAsync(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            return appUser.ToUser();
        }

        public async Task<User> FindByLoginAsync(string loginProvider, string providerKey)
        {
            var appUser = await _userManager.FindByLoginAsync(loginProvider, providerKey);
            return appUser.ToUser();
        }

        public async Task<bool> CheckPassword(User user, string password)
        {
            var appUser = user.ToApplicationUser();
            return await _userManager.CheckPasswordAsync(appUser, password);
        }

        public async Task<SetUserEmailResponse> SetEmailAsync(User user, string email)
        {
            var appUser = user.ToApplicationUser();
            var identityResult = await _userManager.SetEmailAsync(appUser, email);
            return new SetUserEmailResponse(email,identityResult.Succeeded,
                identityResult.Errors.Select(e => new Error(e.Code, e.Description)));
        }
    }
}