using System.Linq;
using Common.Utils.Extensions;
using Microsoft.Extensions.Options;
using OnlineStore.Modules.Identity.Domain.Configurations.Options;
using OnlineStore.Modules.Identity.Domain.Users.Services;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Services
{
    public class UserEditable : IUserEditable
    {
        private readonly AuthorizationOptions _securityOptions;
        public UserEditable(IOptions<AuthorizationOptions> securityOptions)
        {
            _securityOptions = securityOptions.Value;
        }
        public bool IsUserEditable(string userName)
        {
            return _securityOptions.NonEditableUsers?.FirstOrDefault(x => x.EqualsInvariant(userName)) == null;
        }
    }
}