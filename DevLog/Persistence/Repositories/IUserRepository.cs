using DevLog.Core.Domain;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DevLog.Persistence.Repositories
{
    /// <summary>
    /// Represents user repository interface
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>All users as IQueryable</returns>
        IQueryable<User> GetAll();

        /// <summary>
        /// Get a user by its identifier
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <returns>A specific user or null</returns>
        Task<User?> GetById(int id);

        /// <summary>
        /// Get user by ClaimsPrincipal
        /// </summary>
        /// <param name="principal">Claims principal</param>
        /// <returns>A user with the given claims principal or null</returns>
        Task<User?> GetByClaimsPrincipal(ClaimsPrincipal principal);

        /// <summary>
        /// Get a user by its username
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>A user with the given username or null</returns>
        Task<User?> GetByUserName(string userName);

        /// <summary>
        /// Get a user by its email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>A specific user or null</returns>
        Task<User?> GetByEmail(string email);

        /// <summary>
        /// Insert a new user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> Insert(User user, string password);

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="newPassword">New password</param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> Update(User user, string? newPassword = null);

        /// <summary>
        /// Sign out the current user
        /// </summary>
        Task SignOut();

        /// <summary>
        /// Sign in user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="isPersistent">Is persistent</param>
        Task SignIn(User user, bool isPersistent);

        /// <summary>
        /// Sign in user with password
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        /// <param name="isPersistent">Is persistent</param>
        /// <param name="lookoutOnFailure">Look out on failure</param>
        /// <returns>SignInResult</returns>
        Task<SignInResult> SignInByPassword(User user, string password, bool isPersistent, bool lookoutOnFailure);

        /// <summary>
        /// Add user to a role
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="role">Role</param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> AddToRole(User user, string role);

        /// <summary>
        /// Generate user password reset token
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>User password reset token</returns>
        Task<string> GeneratePasswordResetToken(User user);

        /// <summary>
        /// Reset user password
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="token">Password reset token</param>
        /// <param name="newPassword">New password</param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> ResetPassword(User user, string token, string newPassword);

        /// <summary>
        /// Check if user has a role
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="role">Role</param>
        /// <returns>True if user is in the role; otherwise, false</returns>
        Task<bool> IsInRole(User user, string role);

        /// <summary>
        /// Check if user is allowed for the operation
        /// </summary>
        /// <param name="currentUser">Current user</param>
        /// <param name="operationUserId">User who does operation</param>
        /// <param name="alwaysAllowedRole">Role that always allowed for the operation</param>
        /// <returns>True if user allowed to do operation; otherwise, false</returns>
        Task<bool> IsUserAllowedForOperation(User currentUser, int operationUserId, string alwaysAllowedRole);

        /// <summary>
        /// Count total users
        /// </summary>
        /// <returns>Number of all users</returns>
        Task<int> Count();
    }
}
