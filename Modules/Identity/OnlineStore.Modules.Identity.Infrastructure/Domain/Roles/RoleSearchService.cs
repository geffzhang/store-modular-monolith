using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Domain;
using Common.Identity.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using VaultSharp.V1.SecretsEngines.Database;

namespace Common.Identity.Services
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

            result.Results = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();

            return result;
        }
    }
}
