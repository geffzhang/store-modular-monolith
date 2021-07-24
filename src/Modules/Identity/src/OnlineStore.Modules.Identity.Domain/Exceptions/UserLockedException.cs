using System;
using BuildingBlocks.Core.Exceptions;

namespace OnlineStore.Modules.Identity.Domain.Exceptions
{
    internal class UserLockedException : DomainException
    {
        public UserLockedException(Guid userId) : base($"User with ID: '{userId}' is locked.")
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}