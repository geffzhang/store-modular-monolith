using System;

namespace Common.Exceptions
{
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message)
        {
        }
    }
}