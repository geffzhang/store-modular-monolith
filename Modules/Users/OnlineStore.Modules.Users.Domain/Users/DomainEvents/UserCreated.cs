using System;
using Common.Domain;
using static Common.Utils.DateTimeHelper;

namespace OnlineStore.Modules.Users.Domain.Users.Events
{
    internal class UserCreated : IDomainEvent
    {
        public UserCreated(UserId userId, string name, string role)
        {
            UserId = userId;
            Name = name;
            Role = role;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CorrelationId { get; set; }
        public UserId UserId { get; }
        public string Name { get; }
        public string Role { get; }
        public DateTime CreatedAt => NewDateTime();
    }
}