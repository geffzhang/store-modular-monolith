using System;

namespace BuildingBlocks.Core.Exceptions
{
    public abstract class AppException : Exception
    {
        public virtual string Code { get; }
        protected AppException(string message) : base(message)
        {
        }
    }
}