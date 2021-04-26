using Common.Exceptions;

namespace OnlineStore.Modules.Users.Domain.Exceptions
{
    internal class NameInUseException : DomainException
    {
        public NameInUseException(string name) : base($"Name {name} is already in use.")
        {
            Name = name;
        }

        public string Name { get; }
    }
}