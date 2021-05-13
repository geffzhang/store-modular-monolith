using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Messaging.Queries;
using Common.Utils.Reflection;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Modules.Identity.Application.Users.Dtos;

namespace OnlineStore.Modules.Identity.Application.Users.SearchUsers
{
    public class SearchUsersQueryHandler : IQueryHandler<SearchUsersQuery, UserSearchResponse>
    {
        private readonly IMapper _mapper;
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public SearchUsersQueryHandler(IMapper mapper,Func<UserManager<ApplicationUser>> userManager)
        {
            _mapper = mapper;
            _userManagerFactory = userManager;
        }


        public async Task<UserSearchResponse> HandleAsync(SearchUsersQuery query)
        {
            var search = _mapper.Map<UserSearchCriteriaDto>(query);

            using (var userManager = _userManagerFactory())
            {
                if (query == null)
                {
                    throw new ArgumentNullException(nameof(query));
                }

                if (!userManager.SupportsQueryableUsers)
                {
                    throw new NotSupportedException();
                }

                var result = TypeFactory<UserSearchResponse>.TryCreateInstance();
                var query = userManager.Users;
                if (query.Keyword != null)
                {
                    query = query.Where(x => x.UserName.Contains(query.Keyword));
                }

                if (!string.IsNullOrEmpty(query.MemberId))
                {
                    query = query.Where(x => x.MemberId == query.MemberId);
                }

                if (!query.MemberIds.IsNullOrEmpty())
                {
                    query = query.Where(x => query.MemberIds.Contains(x.MemberId));
                }

                if (query.ModifiedSinceDate != null && query.ModifiedSinceDate != default(DateTime))
                {
                    query = query.Where(x => x.ModifiedDate > query.ModifiedSinceDate);
                }

                result.TotalCount = await query.CountAsync();

                var res = await query.Skip(query.Skip).Take(query.Take).ToArrayAsync();
                result.Results = res.Select(user => user.ToUser()).ToList();
                return result;
            }
        }
    }
}