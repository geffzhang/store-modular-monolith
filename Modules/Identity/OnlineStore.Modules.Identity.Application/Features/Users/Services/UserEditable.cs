using System.Linq;
using Common.Utils.Extensions;
using OnlineStore.Modules.Identity.Application.Contracts;
using OnlineStore.Modules.Identity.Domain.Configurations.Options;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Domain.Users.Services;

namespace OnlineStore.Modules.Identity.Application.Users.Services
{
    public class UserEditable : IUserEditable
    {
        private readonly AuthorizationOptions _securityOptions;
        public UserEditable(AuthorizationOptions securityOptions)
        {
            _securityOptions = securityOptions;
        }
        public bool IsUserEditable(string userName)
        {
            return _securityOptions.NonEditableUsers?.FirstOrDefault(x => x.EqualsInvariant(userName)) == null;
        }
    }
}