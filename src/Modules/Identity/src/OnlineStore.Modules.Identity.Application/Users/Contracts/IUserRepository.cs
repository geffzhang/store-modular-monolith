using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BuildingBlocks.Core.Persistence;
using OnlineStore.Modules.Identity.Application.Users.Dtos.GatewayResponses.Repositories;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Types;

namespace OnlineStore.Modules.Identity.Application.Users.Contracts
{
    public interface IUserRepository : IRepository<User, Guid, UserId>
    {
        Task<CreateUserResponse> AddAsync(User user);
        Task<UpdateUserResponse> UpdateAsync(User user);
        Task<User> FindByNameOrEmailAsync(string userNameOrEmail);
        Task<User> FindByNameAsync(string userName);
        Task<User> FindByIdAsync(string id);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByLoginAsync(string loginProvider, string providerKey);
        Task<bool> CheckPassword(User user, string password);
        Task<SetUserEmailResponse> SetEmailAsync(User user, string email);

        Task<(IList<Claim> UserClaims, IList<string> Roles, IList<Claim> PermissionClaims)>
            GetClaimsAsync(User user);
    }
}