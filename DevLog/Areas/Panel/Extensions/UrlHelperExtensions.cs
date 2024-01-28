using DevLog.Areas.Panel.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DevLog.Areas.Panel.Extensions
{
    /// <summary>
    /// Url helper extensions
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// User password reset link generator
        /// </summary>
        /// <param name="urlHelper">Url helper</param>
        /// <param name="userId">User id</param>
        /// <param name="token">Reset token</param>
        /// <param name="scheme">Website scheme</param>
        /// <returns>Password reset link</returns>
        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, int userId, string token, string scheme)
        {
            return urlHelper.Action(
                action: nameof(UsersController.ResetPassword),
                controller: "Users",
                values: new { userId, token },
                protocol: scheme) ?? string.Empty;
        }
    }
}
