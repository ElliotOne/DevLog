using System.Security.Claims;

namespace DevLog.Areas.Panel.Extensions
{
    /// <summary>
    /// Claim principal extensions
    /// </summary>
    public static class ClaimPrincipalExtensions
    {
        /// <summary>
        /// Get current user identifier
        /// </summary>
        /// <param name="principal">claim principal</param>
        /// <returns>Current user identifier</returns>
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            return principal == null
                ? throw new ArgumentNullException(nameof(principal))
                : (principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
