using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Contracts.Identity;
using YK.Auth.Domain.Entities.Common.Identity;
using YK.Auth.Infrastructure.Abstractions.Persistence.Contexts;

namespace YK.Auth.Infrastructure.Common.Abstractions.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public IdentityService(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> UserExistsAsync(string phone, string role)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);

            if (user is null)
            {
                return false;
            }

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> CreateUserAsync(CreateUserRequest registerRequest)
        {
            var user = new User
            {
                UserName = Guid.NewGuid().ToString(),
                PhoneNumber = registerRequest.Phone,
                Email = registerRequest.Email
            };

            var userResult = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!userResult.Succeeded)
            {
                return (false, userResult.Errors.Select(x => x.Description));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, registerRequest.Role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return (false, roleResult.Errors.Select(x => x.Description));
            }

            return (true, []);
        }

        public async Task<User?> ValidateCredentialsAsync(string phone, string role, string password)
        {
            var normalizedRole = _userManager.NormalizeName(role);

            var users = await (
                from u in _context.Users
                join ur in _context.UserRoles on u.Id equals ur.UserId
                join r in _context.Roles on ur.RoleId equals r.Id
                where u.PhoneNumber == phone && r.NormalizedName == normalizedRole
                select u
            ).ToListAsync();

            foreach (var user in users)
            {
                if (await _userManager.CheckPasswordAsync(user, password))
                    return user;
            }

            return null;
        }
    }
}
