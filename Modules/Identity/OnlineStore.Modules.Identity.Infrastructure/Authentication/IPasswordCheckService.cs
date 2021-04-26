using System.Threading.Tasks;

namespace Common.Identity
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
