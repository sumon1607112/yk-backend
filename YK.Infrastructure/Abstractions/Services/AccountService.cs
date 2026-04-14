using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YK.Application.Abstractions.Services;
using YK.Domain.Entities.Account;

namespace YK.Infrastructure.Abstractions.Services
{

    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<bool> UserExistsAsync(string phone)
        {
            return await _userManager.FindByNameAsync(phone) is not null;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> CreateUserAsync(string phone, string password, string role)
        {
            var user = new User
            {
                UserName = phone,
                PhoneNumber = phone,
                PhoneNumberConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            return (true, []);
        }

        public async Task<string> GenerateTokenAsync(string phone)
        {
            var user =  await _userManager.FindByNameAsync(phone)?? throw new InvalidOperationException("User not found.");
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.PhoneNumber!)
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<bool> ValidateCredentialsAsync(string phone, string password)
        {
            throw new NotImplementedException();
        }
    }
}
