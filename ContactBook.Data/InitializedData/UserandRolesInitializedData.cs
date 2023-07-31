using ContactBook.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Data.InitializedData
{
    public class UserandRolesInitializedData
    {
        public static async Task SeedData(ContactBookContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();
            await SeedRoles(roleManager);
            await SeedUsers(userManager, context);
        }

        private static async Task SeedUsers(UserManager<AppUser> userManager, ContactBookContext context)
        {
            await CreateAndAssignUser(userManager, "David", "Ogwuche", "dvd@gmail.com", "Ysb123@32", "2349064056077", "Makurdi", "Benue", "Nigeria", "User");
            await CreateAndAssignUser(userManager, "Michael", "Batowei", "codedvd@gmail.com", "Ysb123@32", "+2349018015592", "PH", "Porthacort", "Nigeria", "Admin");
        }

        private static async Task CreateAndAssignUser(UserManager<AppUser> userManager, string firstName, string lastName, string email, string password, string phoneNumber, string city, string state, string country, string role)
        {
            if (!userManager.Users.Any(u => u.Email == email))
            {
                var user = new AppUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    ImageUrl = "openForCorrection",
                    FacebookUrl = "facebookurl",
                    TwitterUrl = "twitterurl",
                    UserName = email,
                    PhoneNumber = phoneNumber,
                    City = city,
                    State = state,
                    Country = country
                };

                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.RoleExistsAsync("Admin").Result == false)
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };

                await roleManager.CreateAsync(role);
            }
            if (roleManager.RoleExistsAsync("User").Result == false)
            {
                var role = new IdentityRole
                {
                    Name = "User"
                };

                await roleManager.CreateAsync(role);
            }
        }
    }
}
