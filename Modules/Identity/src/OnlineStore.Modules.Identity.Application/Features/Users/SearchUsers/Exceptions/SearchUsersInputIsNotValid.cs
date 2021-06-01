using Common.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Features.Users.SearchUsers.Exceptions
{
    public class SearchUsersInputIsNotValid : AppException
    {
        public SearchUsersInputIsNotValid(string input) : base($"input value '{input}' is not valid for searching users.")
        {
        }
    }
}