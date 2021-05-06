using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Modules.Identity.Application.Search.Dtos;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles.Mappings;
using OnlineStore.Modules.Identity.Infrastructure.Search;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Roles.Services
{
    public class RoleSearchService : IRoleSearchService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleSearchService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<RoleSearchResult> SearchRolesAsync(RoleSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            if (!_roleManager.SupportsQueryableRoles)
            {
                throw new NotSupportedException();
            }

            var result = AbstractTypeFactory<RoleSearchResult>.TryCreateInstance();
            var query = _roleManager.Roles;
            if (criteria.Keyword != null)
            {
                query = query.Where(r => r.Name.Contains(criteria.Keyword));
            }

            result.TotalCount = await query.CountAsync();
            var res = await query.Skip(criteria.Skip).Take(criteria.Take).ToListAsync();
            result.Results = res.Select(roleApp => roleApp.ToRole()).ToList();

            return result;
        }
    }
}