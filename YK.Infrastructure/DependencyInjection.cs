using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YK.Application.Abstractions.Persistence;
using YK.Infrastructure.Abstractions.Persistence.Contexts;
using YK.Infrastructure.Abstractions.Persistence.Repositories;

namespace YK.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
