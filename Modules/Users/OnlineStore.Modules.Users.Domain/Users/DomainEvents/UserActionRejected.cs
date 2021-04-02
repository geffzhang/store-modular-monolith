using System;
using Common.Messaging;
using static Common.Utils.DateTimeHelper;

namespace OnlineStore.Modules.Users.Domain.Users.DomainEvents
{
    internal class UserActionRejected : IActionRejected
    {
        public UserActionRejected(UserId userId, string code, string reason)
        {
            UserId = userId;
            Code = code;
            Reason = reason;
        }

        public UserId UserId { get; }
        public DateTime CreatedAt => NewDateTime();
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string Code { get; }
        public string Reason { get; }
    }
}