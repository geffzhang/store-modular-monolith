using Common.Web.Contexts;
using OnlineStore.Modules.Identity.Domain.Users.Services;

namespace OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Services
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
