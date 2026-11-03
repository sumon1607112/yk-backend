using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YK.Auth.Application.Common.Abstractions.Services.Phone;
using YK.Auth.Domain.Entities.Common.Identity;
using YK.Auth.Domain.Enums;
using YK.Auth.Infrastructure.Identity;

namespace YK.Auth.Infrastructure.Persistence.Seeds
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var phoneNumberService = services.GetRequiredService<IPhoneNumberService>();

            // 1. Roles: Admin, Seller, Buyer
            foreach (var roleName in Enum.GetNames<RoleEnum>())
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole { Name = roleName });

                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to create role '{roleName}': {errors}");
                    }
                }
            }

            // 2. First admin (skipped if not configured)
            var phone = configuration["Seed:Admin:Phone"];
            var password = configuration["Seed:Admin:Password"];

            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
                return;

            var e164Phone = phoneNumberService.NormalizeOrThrow(phone, "Seed:Admin:Phone is not a valid phone number.");
            var userName = AccountUserName.Create(e164Phone, nameof(RoleEnum.Admin));

            var admin = await userManager.FindByNameAsync(userName);

            if (admin is null)
            {
                admin = new User
                {
                    UserName = userName,
                    PhoneNumber = e164Phone
                };

                var result = await userManager.CreateAsync(admin, password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Admin seed failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, nameof(RoleEnum.Admin)))
            {
                var adminRoleResult = await userManager.AddToRoleAsync(admin, nameof(RoleEnum.Admin));
                if (!adminRoleResult.Succeeded)
                    throw new InvalidOperationException("Admin role assignment failed: " + string.Join(", ", adminRoleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
