using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Messaging.Queries;
using Common.Utils.Extensions;
using Common.Utils.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Modules.Identity.Application.Features.Users.Exceptions;
using OnlineStore.Modules.Identity.Application.Features.Users.SearchUsers;
using OnlineStore.Modules.Identity.Application.Features.Users.SearchUsers.Exceptions;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Mappings;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.SearchUser
{
    public class SearchUsersQueryHandler : IQueryHandler<SearchUsersQuery, UserSearchResponse>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public SearchUsersQueryHandler(Func<UserManager<ApplicationUser>> userManager)
        {
            _userManagerFactory = userManager;
        }

        public async Task<UserSearchResponse> HandleAsync(SearchUsersQuery search)
        {
            using var userManager = _userManagerFactory();
            if (search == null)
            {
                throw new SearchUsersInputIsNotValid(nameof(search));
            }

            if (!userManager.SupportsQueryableUsers)
            {
                throw new QueryOnUsersIsNotSupport();
            }

            var result = TypeFactory<UserSearchResponse>.TryCreateInstance();
            var query = userManager.Users;
            if (search.Keyword != null)
            {
                query = query.Where(x => x.UserName.Contains(search.Keyword));
            }

            if (search.ModifiedSinceDate != null && search.ModifiedSinceDate != default(DateTime))
            {
                query = query.Where(x => x.ModifiedDate > search.ModifiedSinceDate);
            }

            result.TotalCount = await query.CountAsync();

            var res = await query.Skip(search.Skip).Take(search.Take).ToArrayAsync();
            result.Results = res.Select(user => user.ToUserDto()).ToList();

            return result;
        }
    }
}