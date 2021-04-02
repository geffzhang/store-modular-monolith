using System;

namespace OnlineStore.Modules.Users.Application.Authentication
{
    internal interface ITokenStorage
    {
        void Set(Guid commandId, AuthDto token);
        AuthDto Get(Guid commandId);
    }
}