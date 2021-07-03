using Microsoft.AspNetCore.Identity;
using Serilog;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class IdentityResultExtensions
    {
        public static IdentityResult LogResult(this IdentityResult identityResult, string successMessage)
        {
            if (identityResult.Succeeded)
                Log.Logger.Information(successMessage);
            else
            {
                foreach (var error in identityResult.Errors)
                {
                    Log.Logger.Error(error.Description);
                }
            }

            return identityResult;
        }
    }
}