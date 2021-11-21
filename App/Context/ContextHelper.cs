using System;
using App.Models;
using System.Linq;
using App.Context;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace App.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<QuestionsContext>();

                context.Database.EnsureCreated();
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                /* Create roles */
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Admin
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                // Moderator
                if (!await roleManager.RoleExistsAsync("Moderator"))
                    await roleManager.CreateAsync(new IdentityRole("Moderator"));

                // User
                if (!await roleManager.RoleExistsAsync("User"))
                    await roleManager.CreateAsync(new IdentityRole("User"));

                /* Create users */
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

                // Default admin
                string adminUserEmail = "admin@gmail.com";
                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if(adminUser == null)
                {
                    var newAdminUser = new User()
                    {
                        UserName = "admin",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAdminUser, "Qwerty112!");
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
                // Default moderator
                string moderatorEmail = "moderator@gmail.com";
                var moderatorUser = await userManager.FindByEmailAsync(moderatorEmail);
                if(moderatorUser == null)
                {
                    var newModeratorUser = new User()
                    {
                        UserName = "moderator",
                        Email = moderatorEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newModeratorUser, "Qwerty112!");
                    await userManager.AddToRoleAsync(newModeratorUser, "Moderator");
                }

                // Default user
                string appUserEmail = "user@gmail.com";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new User()
                    {
                        UserName = "user",
                        Email = appUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAppUser, "Qwerty112!");
                    await userManager.AddToRoleAsync(newAppUser, "User");
                }
            }
        }
    }
}
