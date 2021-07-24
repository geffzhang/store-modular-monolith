using System;
using OnlineStore.Modules.Identity.Infrastructure.Aggregates.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Authentication.Login
{
    public static class PasswordExpiryHelper
    {
        public static int DaysTillPasswordExpiry(ApplicationUser user)
        {
            var result = -1; // not a valid expiry days number

            if (!user.PasswordExpired)
            {
                var lastPasswordChangeDate = user.LastPasswordChangedDate ?? user.CreatedDate;
                var timeTillExpiry = lastPasswordChangeDate - DateTime.UtcNow;
                if (timeTillExpiry > TimeSpan.Zero)
                {
                    result = timeTillExpiry.Days;
                }
            }

            return result;
        }
    }
}