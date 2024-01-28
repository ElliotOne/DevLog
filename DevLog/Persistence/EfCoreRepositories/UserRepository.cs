using DevLog.Core.Domain;
using DevLog.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevLog.Persistence.EfCoreRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserRepository(
            UserManager<User> userManager,
            SignInManager<User> singInManager)
        {
            _userManager = userManager;
            _signInManager = singInManager;
        }

        public IQueryable<User> GetAll()
        {
            return _userManager.Users.AsQueryable();
        }

        public async Task<User?> GetById(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<User?> GetByClaimsPrincipal(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }

        public async Task<User?> GetByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> Insert(User user, string password)
        {
            user.CreateDate = user.LastEditDate = DateTime.Now;
            user.EmailConfirmed = user.PhoneNumberConfirmed = true;

            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> Update(User user, string? newPassword)
        {
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                string token = await GeneratePasswordResetToken(user);
                var result = await ResetPassword(user, token, newPassword);

                if (!result.Succeeded)
                {
                    return IdentityResult.Failed();
                }
            }

            user.LastEditDate = DateTime.Now;
            user.EmailConfirmed = user.PhoneNumberConfirmed = true;

            return await _userManager.UpdateAsync(user);
        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task SignIn(User user, bool isPersistent)
        {
            await _signInManager.SignInAsync(user, isPersistent);
        }

        public async Task<SignInResult> SignInByPassword(User user, string password, bool isPersistent, bool lookoutOnFailure)
        {
            return await _signInManager.PasswordSignInAsync(user, password, isPersistent, lookoutOnFailure);
        }

        public async Task<IdentityResult> AddToRole(User user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<string> GeneratePasswordResetToken(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPassword(User user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<bool> IsInRole(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> IsUserAllowedForOperation(User currentUser, int operationUserId, string alwaysAllowedUserRole)
        {
            return operationUserId == currentUser.Id || await IsInRole(currentUser, alwaysAllowedUserRole);
        }

        public async Task<int> Count()
        {
            return await _userManager.Users.CountAsync();
        }
    }
}
