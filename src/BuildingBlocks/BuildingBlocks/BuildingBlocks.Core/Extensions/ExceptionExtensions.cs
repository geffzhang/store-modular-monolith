using System;
using Humanizer;

namespace BuildingBlocks.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetExceptionCode(this Exception exception)
        {
            return exception.GetType().Name.Underscore().Replace("_exception", string.Empty);
        }
    }
}