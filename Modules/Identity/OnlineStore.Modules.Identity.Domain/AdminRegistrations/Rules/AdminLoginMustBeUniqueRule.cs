using Common.Domain;
using OnlineStore.Modules.Identity.Domain.UserRegistrations.DomainServices;

namespace OnlineStore.Modules.Identity.Domain.AdminRegistrations.Rules
{
    public class AdminLoginMustBeUniqueRule : IBusinessRule
    {
        private readonly string _login;
        private readonly IUsersCounter _usersCounter;

        internal AdminLoginMustBeUniqueRule(string login, IUsersCounter usersCounter)
        {
            _login = login;
            _usersCounter = usersCounter;
        }

        public string Message => "Admin Login must be unique";

        public bool IsBroken()
        {
            return _usersCounter.CountUsersWithLogin(_login) > 0;
        }
    }
}