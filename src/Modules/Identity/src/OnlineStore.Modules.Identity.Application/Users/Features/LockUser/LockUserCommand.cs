using System;
using BuildingBlocks.Cqrs.Commands;

namespace OnlineStore.Modules.Identity.Application.Users.Features.LockUser
{
    public class LockUserCommand : ICommand
    {
        public Guid Id { get; }
    }
}