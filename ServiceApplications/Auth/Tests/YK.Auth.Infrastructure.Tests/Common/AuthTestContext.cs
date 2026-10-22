using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YK.Auth.Domain.Entities.Common.Identity;
using YK.Auth.Infrastructure.Identity;
using YK.Auth.Infrastructure.Persistence;

namespace YK.Auth.Infrastructure.Tests.Common
{
    /// <summary>
    /// Gives each test its own empty in-memory database and real services.
    /// Use with: await using var ctx = await AuthTestContext.CreateAsync();
    /// </summary>
    public sealed class AuthTestContext : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ServiceProvider _provider;
        private readonly List<AsyncServiceScope> _scopes = new();

        private AuthTestContext(SqliteConnection connection, ServiceProvider provider)
        {
            _connection = connection;
            _provider = provider;
        }

        public static async Task<AuthTestContext> CreateAsync()
        {
            // The in-memory database lives as long as this connection stays open
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:SigningAlgorithm"] = "HS256",
                    ["Jwt:Hs256:SecretKey"] = "test-secret-key-that-is-at-least-32-characters",
                    ["Jwt:Issuer"] = "yk-tests",
                    ["Jwt:Audience"] = "yk-tests",
                    ["Jwt:AccessTokenMinutes"] = "15",
                    ["Jwt:RefreshTokenDays"] = "7"
                })
                .Build();

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));

            services.AddIdentityCore<User>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<TokenService>();

            var provider = services.BuildServiceProvider();

            // Create the tables
            await using (var scope = provider.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await db.Database.EnsureCreatedAsync();
            }

            return new AuthTestContext(connection, provider);
        }

        /// <summary>A TokenService in its own scope, like a separate HTTP request.</summary>
        public TokenService CreateTokenService()
        {
            var scope = _provider.CreateAsyncScope();
            _scopes.Add(scope);
            return scope.ServiceProvider.GetRequiredService<TokenService>();
        }

        public async Task<User> CreateUserAsync(string phone = "01700000000")
        {
            await using var scope = _provider.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var user = new User
            {
                UserName = Guid.NewGuid().ToString("N"),
                PhoneNumber = phone
            };

            var result = await userManager.CreateAsync(user, "Passw0rd!");
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return user;
        }

        /// <summary>Reads tokens straight from the database with a fresh DbContext.</summary>
        public async Task<List<RefreshToken>> GetRefreshTokensAsync(string userId)
        {
            await using var scope = _provider.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            return await db.RefreshTokens
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var scope in _scopes)
                await scope.DisposeAsync();

            await _provider.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}