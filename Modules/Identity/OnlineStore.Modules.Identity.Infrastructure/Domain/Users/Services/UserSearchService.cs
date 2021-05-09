using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Domain;
using Common.Utils.Extensions;
using Common.Utils.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Modules.Identity.Application.Search.Dtos;
using OnlineStore.Modules.Identity.Application.Users.Dtos;
using OnlineStore.Modules.Identity.Application.Users.Services;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Mappings;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Services
{
    public class UserSearchService : IUserSearchService
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public UserSearchService(Func<UserManager<ApplicationUser>> userManager)
        {
            _userManagerFactory = userManager;
        }

        public async Task<UserSearchResult> SearchUsersAsync(UserSearchCriteria criteria)
        {
            using (var userManager = _userManagerFactory())
            {
                if (criteria == null)
                {
                    throw new ArgumentNullException(nameof(criteria));
                }

                if (!userManager.SupportsQueryableUsers)
                {
                    throw new NotSupportedException();
                }

                var result = TypeFactory<UserSearchResult>.TryCreateInstance();
                var query = userManager.Users;
                if (criteria.Keyword != null)
                {
                    query = query.Where(x => x.UserName.Contains(criteria.Keyword));
                }

                if (!string.IsNullOrEmpty(criteria.MemberId))
                {
                    query = query.Where(x => x.MemberId == criteria.MemberId);
                }

                if (!criteria.MemberIds.IsNullOrEmpty())
                {
                    query = query.Where(x => criteria.MemberIds.Contains(x.MemberId));
                }

                if (criteria.ModifiedSinceDate != null && criteria.ModifiedSinceDate != default(DateTime))
                {
                    query = query.Where(x => x.ModifiedDate > criteria.ModifiedSinceDate);
                }

                result.TotalCount = await query.CountAsync();

                var res = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                result.Results = res.Select(user => user.ToUser()).ToList();
                return result;
            }
        }
    }
}