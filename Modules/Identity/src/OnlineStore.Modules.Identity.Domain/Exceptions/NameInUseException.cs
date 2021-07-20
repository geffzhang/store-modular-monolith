using Common.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Exceptions
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