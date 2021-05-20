using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common;
using Common.Caching.Caching;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineStore.Modules.Identity.Infrastructure.Caching;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Events;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Mappings;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Services
{
    public class CustomUserManager : AspNetUserManager<ApplicationUser>
    {
        private readonly IExtendedMemoryCache _memoryCache;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public CustomUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger, RoleManager<ApplicationRole> roleManager,
            IExtendedMemoryCache memoryCache, IServiceScopeFactory serviceScopeFactory)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
                services, logger)
        {
            _memoryCache = memoryCache;
            _serviceScopeFactory = serviceScopeFactory;
            _roleManager = roleManager;
        }

        public override async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(FindByLoginAsync), loginProvider, providerKey);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var user = await base.FindByLoginAsync(loginProvider, providerKey);
                if (user != null)
                {
                    await LoadUserDetailsAsync(user);
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeTokenForUser(user));
                }

                return user;
            }, cacheNullValue: false);

            return result;
        }

        public override async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(FindByEmailAsync), email);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var user = await base.FindByEmailAsync(email);
                if (user != null)
                {
                    await LoadUserDetailsAsync(user);
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeTokenForUser(user));
                }

                return user;
            }, cacheNullValue: false);
            return result;
        }

        public override async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(FindByNameAsync), userName);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var user = await base.FindByNameAsync(userName);
                if (user != null)
                {
                    await LoadUserDetailsAsync(user);
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeTokenForUser(user));
                }

                return user;
            }, cacheNullValue: false);
            return result;
        }

        public override async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(FindByIdAsync), userId);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var user = await base.FindByIdAsync(userId);
                if (user != null)
                {
                    await LoadUserDetailsAsync(user);
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeTokenForUser(user));
                }

                return user;
            }, cacheNullValue: false);
            return result;
        }

        public override async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token,
            string newPassword)
        {
            //It is important to call base.FindByIdAsync method to avoid of update a cached user.
            var existUser = await base.FindByIdAsync(user.Id);

            var result = await base.ResetPasswordAsync(existUser, token, newPassword);
            if (result == IdentityResult.Success)
            {
                SecurityCacheRegion.ExpireUser(user);
            }

            return result;
        }

        public override async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword,
            string newPassword)
        {
            var result = await base.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result == IdentityResult.Success)
            {
                SecurityCacheRegion.ExpireUser(user);
            }

            return result;
        }

        public override async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var commandProcessor = scope.ServiceProvider.GetRequiredService<ICommandProcessor>();

            await commandProcessor.PublishDomainEventAsync(new UserChangingDomainEvent(user.ToUser()));
            var result = await base.DeleteAsync(user);
            if (result.Succeeded)
            {
                await commandProcessor.PublishDomainEventAsync(new UserChangedDomainEvent(user.ToUser()));
                SecurityCacheRegion.ExpireUser(user);
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
            var commandProcessor = scope.ServiceProvider.GetRequiredService<ICommandProcessor>();

            await commandProcessor.PublishDomainEventAsync(new UserChangingDomainEvent(user.ToUser()));
            user.Patch(existUser);
            var result = await base.UpdateAsync(existUser);
            if (result.Succeeded)
            {
                await commandProcessor.PublishDomainEventAsync(new UserChangedDomainEvent(user.ToUser()));
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

                SecurityCacheRegion.ExpireUser(existUser);
            }

            return result;
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var commandProcessor = scope.ServiceProvider.GetRequiredService<ICommandProcessor>();

            await commandProcessor.PublishDomainEventAsync(new UserChangingDomainEvent(user.ToUser()));
            var result = await base.CreateAsync(user);
            if (result.Succeeded)
            {
                await commandProcessor.PublishDomainEventAsync(new UserChangedDomainEvent(user.ToUser()));
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

                SecurityCacheRegion.ExpireUser(user);
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