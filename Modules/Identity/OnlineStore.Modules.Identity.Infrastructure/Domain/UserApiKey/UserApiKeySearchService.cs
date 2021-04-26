using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Domain;
using Common.Identity.Search;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Common.Identity.Services
{
    public class UserApiKeySearchService : IUserApiKeySearchService
    {
        private readonly Func<ISecurityRepository> _repositoryFactory;

        public UserApiKeySearchService(Func<ISecurityRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<UserApiKeySearchResult> SearchUserApiKeysAsync(UserApiKeySearchCriteria criteria)
        {
            var repository = _repositoryFactory();
            
                if (criteria == null)
                {
                    throw new ArgumentNullException(nameof(criteria));
                }

                var result = AbstractTypeFactory<UserApiKeySearchResult>.TryCreateInstance();

                var query = repository.UserApiKeys.AsNoTracking();
                result.TotalCount = await query.CountAsync();
                var apiKeysEntities = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                result.Results = apiKeysEntities.Select(x => x.ToModel(AbstractTypeFactory<UserApiKey>.TryCreateInstance())).ToArray();

                return result;
            
        }
    }
}
