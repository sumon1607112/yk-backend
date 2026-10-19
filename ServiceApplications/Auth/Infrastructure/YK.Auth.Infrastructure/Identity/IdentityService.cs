using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Contracts.Identity;
using YK.Auth.Application.Common.Exceptions;
using YK.Auth.Domain.Entities.Common.Identity;
using YK.Auth.Infrastructure.Persistence;

namespace YK.Auth.Infrastructure.Identity
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
            var normalizedRole = _userManager.NormalizeName(role);

            return await UsersByPhoneAndRole(phone, role).AnyAsync();
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

            // 1. Find the user by phone + role
            var user = await UsersByPhoneAndRole(phone, role).FirstOrDefaultAsync();

            if (user is null)
                return null;

            // 2. Locked? Reject even if the password is correct
            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedException("Account is locked. Try again later.");

            // 3. Wrong password? Count the failure
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                await _userManager.AccessFailedAsync(user);
                return null;
            }

            // 4. Correct password: reset the failure count
            await _userManager.ResetAccessFailedCountAsync(user);
            return user;
        }


        //Helper method to get users by phone and role
        private IQueryable<User> UsersByPhoneAndRole(string phone, string role)
        {
            var normalizedRole = _userManager.NormalizeName(role);

            return from u in _context.Users
                   join ur in _context.UserRoles on u.Id equals ur.UserId
                   join r in _context.Roles on ur.RoleId equals r.Id
                   where u.PhoneNumber == phone && r.NormalizedName == normalizedRole
                   select u;
        }
    }
}
