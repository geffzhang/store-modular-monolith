using BuildingBlocks.Web.Contexts;
using OnlineStore.Modules.Identity.Domain.Aggregates.Users.Services;

namespace OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Services
{
    public class UserNameResolver : IUserNameResolver
    {
        private readonly IExecutionContextAccessor _executionContextAccessor;

        public UserNameResolver(IExecutionContextAccessor executionContextAccessor)
        {
            _executionContextAccessor = executionContextAccessor;
        }

        public string GetCurrentUserName()
        {
            return _executionContextAccessor.ExecutionContext.IdentityContext.Id;
        }
    }
}
