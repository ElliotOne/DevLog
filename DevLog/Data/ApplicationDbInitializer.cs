using DevLog.Core.Domain;
using Microsoft.AspNetCore.Identity;

namespace DevLog.Data
{
    /// <summary>
    /// Represents application database initializer
    /// </summary>
    public static class ApplicationDbInitializer
    {
        private static ApplicationDbContext _context = null!;
        private static List<string> _roleNames = new List<string>();
        private static string _superUserRole = string.Empty;
        private static User _user = new User();
        private static string _userPassword = string.Empty;

        public static void SeedData(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            List<string> roleNames,
            string superUserRole,
            User user,
            string userPassword
            )
        {
            _context = context;
            _roleNames = roleNames;
            _superUserRole = superUserRole;
            _user = user;
            _userPassword = userPassword;
            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedDatabase();
        }

        private static void SeedRoles(RoleManager<Role> roleManager)
        {
            foreach (var roleName in _roleNames)
            {
                if (!roleManager.RoleExistsAsync(roleName).Result)
                {
                    Role role = new Role()
                    {
                        Name = roleName
                    };

                    _ = roleManager.CreateAsync(role).Result;
                }
            }
        }

        private static void SeedUsers(UserManager<User> userManager)
        {
            var adminCount = userManager.GetUsersInRoleAsync(_superUserRole).Result.Count;

            if (adminCount <= 0)
            {
                if (userManager.FindByNameAsync(_user.UserName).Result == null)
                {
                    var result = userManager.CreateAsync
                        (_user, _userPassword).Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(_user, _superUserRole).Wait();
                    }
                }
            }
        }

        private static void SeedDatabase()
        {
            if (!_context.Settings.Any())
            {
                _context.Settings.Add(new Setting());
                _context.SaveChanges();
            }
        }
    }
}
