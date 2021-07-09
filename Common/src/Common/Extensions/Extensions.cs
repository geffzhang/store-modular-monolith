using System;
using Humanizer;

namespace Common.Extensions
{
    public static class Extensions
    {
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }

        public static string GetModuleName(this object value)
        {
            return value?.GetType().GetModuleName() ?? string.Empty;
        } 

        public static string GetExceptionCode(this Exception exception)
        {
            return exception.GetType().Name.Underscore().Replace("_exception", string.Empty);
        }

        public static Guid BindId(this Guid id)
        {
            return id == Guid.Empty ? Guid.NewGuid() : id;
        }
    }
}