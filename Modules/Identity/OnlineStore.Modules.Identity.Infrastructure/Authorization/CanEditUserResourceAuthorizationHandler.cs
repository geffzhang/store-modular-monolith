using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OnlineStore.Modules.Identity.Application.Contracts;
using OnlineStore.Modules.Identity.Domain.Users;

namespace OnlineStore.Modules.Identity.Infrastructure.Authorization
{
    public class CanEditOrganizationResourceAuthorizeRequirement : IAuthorizationRequirement
    {
        public const string PolicyName = "CanEditOrganizationResource";
    }

    public class CanEditUserResourceAuthorizationHandler : AuthorizationHandler<CanEditOrganizationResourceAuthorizeRequirement, User>
    {
        private readonly IOnlineStoreContextAccessor _onlineStoreContextAccessor;

        public CanEditUserResourceAuthorizationHandler(IOnlineStoreContextAccessor onlineStoreContextAccessor)
        {
            _onlineStoreContextAccessor = onlineStoreContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CanEditOrganizationResourceAuthorizeRequirement requirement, User resource)
        {
            var onlineStoreContext = _onlineStoreContextAccessor.OnlineStoreContext;
            //Allow to do all things with self 

            var currentUserOrgId = onlineStoreContext.CurrentUser.Id;
            var result = currentUserOrgId != null && resource != null && currentUserOrgId == resource.Id;

            if (result) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}