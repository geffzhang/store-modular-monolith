using Common.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Features.Users.Exceptions
{
    public class UserNameAlreadyInUseException : AppException
    {
        public string Name { get; }

        public UserNameAlreadyInUseException(string name) : base($"UserName '{name}' is already in use.")
        {
            Name = name;
        }
    }
}