using System;
using BuildingBlocks.Cqrs.Commands;

namespace OnlineStore.Modules.Identity.Application.Users.Features.UnlockUser
{
    public class UnlockUserCommand : ICommand
    {
        public Guid Id { get; }
    }
}