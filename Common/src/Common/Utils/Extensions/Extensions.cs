using System;
using System.Linq;

namespace Common.Utils.Extensions
{
    public static class Extensions
    {
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }

        public static string Underscore(this string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
                    .ToLowerInvariant();
        }

        public static string GetModuleName(this object value)
        {
            return value?.GetType().GetModuleName() ?? string.Empty;
        }

        public static string GetModuleName(this Type type)
        {
            if (type?.Namespace is null) return string.Empty;
            var moduleName = type.Assembly.GetModuleName();
            return type.Namespace.StartsWith(moduleName)
                ? type.Namespace.Split(".")[2].ToLowerInvariant()
                : string.Empty;
        }

        public static string GetExceptionCode(this Exception exception)
        {
            return exception.GetType().Name.Underscore().Replace("_exception", string.Empty);
        }
    }
}