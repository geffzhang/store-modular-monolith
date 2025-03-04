using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Core.Utils.Reflection;
using BuildingBlocks.Cqrs.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Modules.Identity.Application.Users.Exceptions;
using OnlineStore.Modules.Identity.Application.Users.Features.SearchUsers;
using OnlineStore.Modules.Identity.Application.Users.Features.SearchUsers.Exceptions;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Features.SearchUser
{
    public class SearchUsersQueryHandler : IQueryHandler<SearchUsersQuery, UserSearchResponse>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public SearchUsersQueryHandler(Func<UserManager<ApplicationUser>> userManager)
        {
            _userManagerFactory = userManager;
        }

        public async Task<UserSearchResponse> HandleAsync(SearchUsersQuery search,
            CancellationToken cancellationToken = default)
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

            result.TotalCount = await query.CountAsync(cancellationToken);

            var res = await query.Skip(search.Skip).Take(search.Take).ToArrayAsync(cancellationToken);
            result.Results = res.Select(user => user.ToUserDto()).ToList();

            return result;
        }
    }
}