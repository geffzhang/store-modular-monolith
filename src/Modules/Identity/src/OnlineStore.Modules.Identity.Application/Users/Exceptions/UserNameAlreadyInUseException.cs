using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Application.Users.Exceptions
{
    public class UserNameAlreadyInUseException : AppException
    {
        public string Name { get; }

        public UserNameAlreadyInUseException(string name) : base($"UserName '{name}' is already in used.")
        {
            Name = name;
        }
    }
}