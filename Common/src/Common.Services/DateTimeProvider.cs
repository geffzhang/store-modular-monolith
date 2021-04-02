using System;

namespace Common.Services
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Get()
        {
            return DateTime.UtcNow;
        }
    }
}