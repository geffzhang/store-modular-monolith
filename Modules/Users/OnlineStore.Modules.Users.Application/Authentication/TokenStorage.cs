using System;

namespace OnlineStore.Modules.Users.Application.Authentication
{
    internal class TokenStorage : ITokenStorage
    {
        private readonly IRequestStorage _storage;

        public TokenStorage(IRequestStorage storage)
        {
            _storage = storage;
        }

        public void Set(Guid commandId, AuthDto token) => _storage.Set(GetKey(commandId), token);

        public AuthDto Get(Guid commandId) => _storage.Get<AuthDto>(GetKey(commandId));
        
        private static string GetKey(Guid commandId) => $"users:tokens:{commandId}";
    }
}