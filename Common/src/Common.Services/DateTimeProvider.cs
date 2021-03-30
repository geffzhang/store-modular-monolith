using System;

namespace Common.Services
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Get()  => DateTime.UtcNow;
    }
}