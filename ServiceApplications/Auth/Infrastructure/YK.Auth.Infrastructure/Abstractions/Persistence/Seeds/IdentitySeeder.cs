using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YK.Auth.Domain.Entities.Common.Identity;
using YK.Auth.Domain.Enums;

namespace YK.Auth.Infrastructure.Abstractions.Persistence.Seeds
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<User>>();

            // 1. Roles: Admin, Seller, Buyer
            foreach (var roleName in Enum.GetNames<RoleEnum>())
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole { Name = roleName });
            }

            // 2. First admin (skipped if not configured)
            var phone = configuration["Seed:Admin:Phone"];
            var password = configuration["Seed:Admin:Password"];

            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
                return;

            var admins = await userManager.GetUsersInRoleAsync(nameof(RoleEnum.Admin));
            if (admins.Any(u => u.PhoneNumber == phone))
                return;

            var admin = new User
            {
                UserName = Guid.NewGuid().ToString(),
                PhoneNumber = phone
            };

            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
                throw new InvalidOperationException("Admin seed failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));


            await userManager.AddToRoleAsync(admin, nameof(RoleEnum.Admin));
        }
    }
}
