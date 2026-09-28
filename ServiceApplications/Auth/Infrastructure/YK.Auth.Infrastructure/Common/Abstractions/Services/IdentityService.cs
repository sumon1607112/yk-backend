using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Contracts.Identity;
using YK.Auth.Domain.Entities.Common.Identity;

namespace YK.Auth.Infrastructure.Common.Abstractions.Services
{
    public class IdentityService : IIdentityService
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public IdentityService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
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

        public async Task<bool> ValidateCredentialsAsync(string phone, string password)
        {
            throw new NotImplementedException();
        }
    }
}
