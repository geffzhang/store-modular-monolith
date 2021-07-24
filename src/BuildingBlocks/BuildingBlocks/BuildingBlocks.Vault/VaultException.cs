using System;

namespace BuildingBlocks.Vault
{
    internal sealed class VaultException : Exception
    {
        public VaultException(string key) : this(null, key)
        {
        }

        public VaultException(Exception innerException, string key) : this(string.Empty, innerException, key)
        {
        }

        public VaultException(string message, Exception innerException, string key) : base(message, innerException)
        {
            Key = key;
        }

        public string Key { get; }
    }
}