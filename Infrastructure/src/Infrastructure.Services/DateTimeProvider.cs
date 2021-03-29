using System;

namespace Infrastructure.Services
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Get()  => DateTime.UtcNow;
    }
}