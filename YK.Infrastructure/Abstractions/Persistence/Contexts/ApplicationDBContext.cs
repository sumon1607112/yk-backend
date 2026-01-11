using Microsoft.EntityFrameworkCore;
using YK.Domain.Entities.Users;

namespace YK.Infrastructure.Abstractions.Persistence.Contexts
{
    // 1. You must declare the class here
    public class ApplicationDbContext : DbContext
    {
        // 2. Now the constructor is valid
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 3. Define your Tables (DbSets)
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This automatically finds all classes implementing IEntityTypeConfiguration
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("User");
        }
    }
}