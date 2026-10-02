using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YK.Auth.Domain.Entities.Common.Identity;

namespace YK.Auth.Infrastructure.Abstractions.Persistence.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            builder.Entity<User>(e => e.ToTable("User"));
            builder.Entity<IdentityRole>(e => e.ToTable("Role"));
            builder.Entity<IdentityUserRole<string>>(e => e.ToTable("UserRole"));
            builder.Entity<IdentityUserClaim<string>>(e => e.ToTable("UserClaim"));
            builder.Entity<IdentityUserLogin<string>>(e => e.ToTable("UserLogin"));
            builder.Entity<IdentityUserToken<string>>(e => e.ToTable("UserToken"));
            builder.Entity<IdentityRoleClaim<string>>(e => e.ToTable("RoleClaim"));
        }
    }
}