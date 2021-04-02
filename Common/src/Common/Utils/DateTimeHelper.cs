using System;
using System.Diagnostics;

namespace Common.Utils
{
    public static class DateTimeHelper
    {
        [DebuggerStepThrough]
        public static DateTime NewDateTime()
        {
            return DateTimeOffset.Now.UtcDateTime;
        }
    }
}