using System;
using BuildingBlocks.Authentication.Jwt;
using BuildingBlocks.Cqrs.Commands;

namespace OnlineStore.Modules.Identity.Application.Authentication.Features.RefreshToken
{
    public class RefreshTokenCommand : ICommand<JsonWebToken>
    {
        public Guid Id { get; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}