using Common.Core.Domain;
using OnlineStore.Modules.Identity.Domain.Users.Services;

namespace OnlineStore.Modules.Identity.Domain.Users.Rules
{
    public class UserEditableRule : IBusinessRule
    {
        private readonly string _userName;
        private readonly IUserEditable _userEditable;

        public UserEditableRule(string userName, IUserEditable userEditable)
        {
            _userName = userName;
            _userEditable = userEditable;
        }

        public bool IsBroken()
        {
            return !_userEditable.IsUserEditable(_userName);
        }

        public string Message => "User can't be edit.";
    }
}