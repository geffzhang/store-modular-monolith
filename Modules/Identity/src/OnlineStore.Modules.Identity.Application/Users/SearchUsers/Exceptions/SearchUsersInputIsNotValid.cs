using Common.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.SearchUsers.Exceptions
{
    public class SearchUsersInputIsNotValid : AppException
    {
        public SearchUsersInputIsNotValid(string input) : base($"input value '{input}' is not valid for searching users.")
        {
        }
    }
}