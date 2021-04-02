using Common.Domain.Types;

namespace OnlineStore.Modules.Users.Domain.Users.DomainEvents
{
    public class UserCreatedDomainEvent : DomainEventBase
    {
        public UserCreatedDomainEvent(UserId id)
        {
            Id = id;
        }

        public new UserId Id { get; }
    }
}