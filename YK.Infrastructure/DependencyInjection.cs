using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YK.Application.Abstractions.Persistence;
using YK.Application.Abstractions.Services;
using YK.Domain.Entities.Account;
using YK.Infrastructure.Abstractions.Persistence.Contexts;
using YK.Infrastructure.Abstractions.Services;

namespace YK.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Read Railway's DATABASE_URL or fallback to appsettings
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            string connectionString;

            if (!string.IsNullOrEmpty(databaseUrl))
            {
                var uri = new Uri(databaseUrl);
                connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
            }
            else
            {
                connectionString = configuration.GetConnectionString("DefaultConnection")!;
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Identity
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Repositories & Services
            services.AddScoped<IAccountService, AccountService>();

            return services;
        }
    }
}
