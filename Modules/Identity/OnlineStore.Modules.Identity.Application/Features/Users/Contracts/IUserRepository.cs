﻿using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Features.Users.Dtos.GatewayResponses.Repositories;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Contracts
{
    public interface IUserRepository
    {
        Task<CreateUserResponse> AddAsync(User user);
        Task<UpdateUserResponse> UpdateAsync(User user);
        Task<User> FindByNameAsync(string userName);
        Task<User> FindByIdAsync(string id);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByLoginAsync(string loginProvider, string providerKey);
        Task<bool> CheckPassword(User user, string password);
        Task<SetUserEmailResponse> SetEmailAsync(User user, string email);
    }
}