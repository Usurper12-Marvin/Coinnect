using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Coinnect.Models.AllModels;

namespace Coinnect.Data
{

    public static class SeedDataIdentity
    {
        private const string adminUser = "admin";
        private const string adminPassword = "AgentM!7";
        private const string adminEmail = "marvinlubisi2@gmail.com";
        private const string adminRole = "Admin";
        private const string adminFirstName = "Ntsako";
        private const string adminLastName = "Lubisi";
        private const string adminPhone = "0453238564";
        private static DateOnly adminDOB = new DateOnly(2000, 6, 6);

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            CoinnectDbContext coinnectIdentity = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<CoinnectDbContext>();

            if (coinnectIdentity.Database.GetPendingMigrations().Any())
            {
                coinnectIdentity.Database.Migrate();
            }

            UserManager<User> userManager = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<UserManager<User>>();

            RoleManager<IdentityRole> roleManager = app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            if (await userManager.FindByNameAsync(adminUser) == null)
            {
                if (await roleManager.FindByNameAsync(adminRole) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(adminRole));
                }

                if (!await roleManager.RoleExistsAsync("Consultant"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Consultant"));
                }

                if (!await roleManager.RoleExistsAsync("Advisor"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Advisor"));
                }
                User user = new User
                {
                    UserName = adminUser,
                    Email = adminEmail,
                    FirstName = adminFirstName,
                    LastName = adminLastName,
                    DateOfBirth = adminDOB,
                    PhoneNumber = adminPhone
                };

                IdentityResult result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }

            }
        }
    }
}
