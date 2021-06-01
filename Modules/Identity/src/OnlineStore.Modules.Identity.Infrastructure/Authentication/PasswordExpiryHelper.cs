using System;
using OnlineStore.Modules.Identity.Domain.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;

namespace OnlineStore.Modules.Identity.Infrastructure.Authentication
{
    public class PasswordExpiryHelper
    {
        public static int ContDaysTillPasswordExpiry(ApplicationUser user, UserOptionsExtended userOptions)
        {
            var result = -1; // not a valid expiry days number

            if (!user.PasswordExpired &&
                userOptions.RemindPasswordExpiryInDays > 0 &&
                userOptions.MaxPasswordAge != null &&
                userOptions.MaxPasswordAge.Value > TimeSpan.Zero)
            {
                var lastPasswordChangeDate = user.LastPasswordChangedDate ?? user.CreatedDate;
                var timeTillExpiry = lastPasswordChangeDate.Add(userOptions.MaxPasswordAge.Value) - DateTime.UtcNow;
                if (timeTillExpiry > TimeSpan.Zero &&
                    timeTillExpiry < TimeSpan.FromDays(userOptions.RemindPasswordExpiryInDays))
                {
                    result = timeTillExpiry.Days;
                }
            }

            return result;
        }
    }
}