using System.Threading.Tasks;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Authentication.Services
{
    /// <summary>
    /// Service for checking password against password security policies.
    /// </summary>
    public interface IPasswordCheckService
    {
        /// <summary>
        /// Runs password validation.
        /// </summary>
        /// <param name="password">String with password to validate.</param>
        /// <returns>Password validation result.</returns>
        Task<PasswordValidationResult> ValidatePasswordAsync(string password);
    }
}
