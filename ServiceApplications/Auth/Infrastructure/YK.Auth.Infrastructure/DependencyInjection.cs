using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhoneNumbers;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Abstractions.Services.Phone;
using YK.Auth.Domain.Entities.Common.Identity;
using YK.Auth.Infrastructure.Identity;
using YK.Auth.Infrastructure.Persistence;
using YK.Auth.Infrastructure.Phone;

namespace YK.Auth.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // DATABASE_URL holds a full SQL Server connection string when hosted, otherwise fall back to appsettings
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = configuration.GetConnectionString("DefaultConnection")!;
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    b =>
                    {
                        b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);

                        // Retry on transient errors such as 40613 (serverless DB paused/resuming)
                        b.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));

            // Identity
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;

                // Lockout: 5 wrong passwords locks the account for 15 minutes
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
             .AddRoles<IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();

            services.AddOptions<PhoneNumberOptions>()
                    .Bind(configuration.GetSection(PhoneNumberOptions.SectionName))
                    .Validate(o => o.AllowedRegions.Length > 0, "PhoneNumber:AllowedRegions must not be empty.")
                    .ValidateOnStart();

            services.AddSingleton(PhoneNumberUtil.GetInstance());
            services.AddSingleton<IPhoneNumberService, PhoneNumberService>();

            // Services
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
