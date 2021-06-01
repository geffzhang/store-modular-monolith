namespace OnlineStore.Modules.Identity.Domain.Users
{
    /// <summary>
    /// Obsolete. Left due to compatibility issues. Will be removed. Instead of, use: ApplicationUser.EmailConfirmed, ApplicationUser.LockoutEnd.
    /// </summary>
    public enum AccountState
    {
        PendingApproval,
        Approved,
        Rejected
    }
}