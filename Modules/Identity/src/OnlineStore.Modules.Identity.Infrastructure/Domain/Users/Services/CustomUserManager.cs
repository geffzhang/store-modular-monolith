using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common;
using Common.Caching.Caching;
using EasyCaching.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Mappings;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Services
{
    public class CustomUserManager : AspNetUserManager<ApplicationUser>
    {
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public CustomUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger, RoleManager<ApplicationRole> roleManager,
            IEasyCachingProviderFactory cachingFactory, IServiceScopeFactory serviceScopeFactory)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
                services, logger)
        {
            _cachingProvider = cachingFactory.GetCachingProvider("mem");
            _serviceScopeFactory = serviceScopeFactory;
            _roleManager = roleManager;
        }

        public override async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(FindByLoginAsync), loginProvider, providerKey);
            var result = await _cachingProvider.GetAsync(cacheKey, async () =>
            {
                var user = await base.FindByLoginAsync(loginProvider, providerKey);
                if (user != null)
                {
                    await LoadUserDetailsAsync(user);
                }

                return user;
            }, TimeSpan.FromMinutes(10));


            return result.Value!;
        }

        public override async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(FindByEmailAsync), email);
            var result = await _cachingProvider.GetAsync(cacheKey, async () =>
            {
                var user = await base.FindByEmailAsync(email);
                if (user != null)
                {
                    await LoadUserDetailsAsync(user);
                }

                return user;
            }, TimeSpan.FromMinutes(10));

            return result.Value!;
        }

        public override async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(FindByNameAsync), userName);
            var result = await _cachingProvider.GetAsync(cacheKey, async () =>
            {
                var user = await base.FindByNameAsync(userName);
                if (user != null)
                {
                    await LoadUserDetailsAsync(user);
                }

                return user;
            }, TimeSpan.FromMinutes(10));

            return result.Value!;
        }

        public override async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(FindByIdAsync), userId);
            var result = await _cachingProvider.GetAsync(cacheKey, async () =>
            {
                var user = await base.FindByIdAsync(userId);
                if (user != null)
                {
                    await LoadUserDetailsAsync(user);
                }

                return user;
            }, TimeSpan.FromMinutes(10));

            return result.Value!;
        }

        public override async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token,
            string newPassword)
        {
            //It is important to call base.FindByIdAsync method to avoid of update a cached user.
            var existUser = await base.FindByIdAsync(user.Id);

            var result = await base.ResetPasswordAsync(existUser, token, newPassword);

            return result;
        }

        public override async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword,
            string newPassword)
        {
            var result = await base.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result == IdentityResult.Success)
            {
                var cacheKey = CacheKey.With(GetType(), nameof(FindByIdAsync), user.Id);
                await _cachingProvider.RemoveAsync(cacheKey);
            }

            return result;
        }

        public override async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var result = await base.DeleteAsync(user);
            if (result.Succeeded)
            {
            }

            return result;
        }

        public override async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            ApplicationUser existUser = null;
            if (!string.IsNullOrEmpty(user.Id))
            {
                //It is important to call base.FindByIdAsync method to avoid of update a cached user.
                existUser = await base.FindByIdAsync(user.Id);
            }

            if (existUser == null)
            {
                //It is important to call base.FindByNameAsync method to avoid of update a cached user.
                existUser = await base.FindByNameAsync(user.UserName);
            }

            //We cant update not existing user
            if (existUser == null)
            {
                return IdentityResult.Failed(ErrorDescriber.DefaultError());
            }

            await LoadUserDetailsAsync(existUser);

            using var scope = _serviceScopeFactory.CreateScope();

            user.Patch(existUser);
            var result = await base.UpdateAsync(existUser);
            if (result.Succeeded)
            {
                if (user.Roles != null)
                {
                    var targetRoles = (await GetRolesAsync(existUser));
                    var sourceRoles = user.Roles.Select(x => x.Name).ToList();
                    //Add
                    foreach (var newRole in sourceRoles.Except(targetRoles))
                    {
                        await AddToRoleAsync(existUser, newRole);
                    }

                    //Remove
                    foreach (var removeRole in targetRoles.Except(sourceRoles))
                    {
                        await RemoveFromRoleAsync(existUser, removeRole);
                    }
                }
            }

            return result;
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var result = await base.CreateAsync(user);
            if (result.Succeeded)
            {
                var permissions = user.Permissions;
                var roles = user.Roles;


                if (permissions != null && permissions.Any())
                {
                    //Add
                    foreach (var permission in permissions)
                    {
                        await AddClaimAsync(user,
                            new Claim(SecurityConstants.Claims.PermissionClaimType, permission.Name));
                    }
                }

                if (roles != null && roles.Any())
                {
                    //Add
                    foreach (var newRole in roles)
                    {
                        if (await _roleManager.RoleExistsAsync(newRole.Name))
                            await AddToRoleAsync(user, newRole.Name);
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Load detailed user information: Roles, external logins, claims (permissions)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected virtual async Task LoadUserDetailsAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Roles = new List<ApplicationRole>();
            foreach (var roleName in await base.GetRolesAsync(user))
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    user.Roles.Add(role);
                }
            }

            user.Permissions = user.Roles.SelectMany(x => x.Permissions).ToArray();

            // Read associated logins (compatibility with v2)
            var logins = await base.GetLoginsAsync(user);
            user.Logins = logins.Select(x =>
                new ApplicationUserLogin() {LoginProvider = x.LoginProvider, ProviderKey = x.ProviderKey}).ToArray();
        }
    }
}