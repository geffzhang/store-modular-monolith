using System;

namespace BuildingBlocks.Vault
{
    internal sealed class VaultAuthTypeNotSupportedException : Exception
    {
        public VaultAuthTypeNotSupportedException(string authType) : this(string.Empty, authType)
        {
        }

        public VaultAuthTypeNotSupportedException(string message, string authType) : base(message)
        {
            AuthType = authType;
        }

        public string AuthType { get; set; }
    }
}