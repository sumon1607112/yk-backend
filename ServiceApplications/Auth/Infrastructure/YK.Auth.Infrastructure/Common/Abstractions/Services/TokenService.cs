using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Contracts.Authentication;
using YK.Auth.Domain.Entities.Common.Identity;

namespace YK.Auth.Infrastructure.Common.Abstractions.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public TokenService(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<AuthTokensDto> GenerateTokensAsync(User user)
        {
            var signingAlgorithm = _configuration["Jwt:SigningAlgorithm"];

            return signingAlgorithm?.ToUpperInvariant() switch
            {
                "HS256" => await GenerateHs256TokensAsync(user),

                "RS256" => await GenerateRs256TokensAsync(user),

                _ => throw new NotSupportedException($"Signing algorithm '{signingAlgorithm}' is not supported.")

            };
        }

        private async Task<AuthTokensDto> GenerateHs256TokensAsync(User user)
        {
            var secretKey = _configuration["Jwt:Hs256:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }

            var claims = await CreateClaims(user);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = CreateToken(claims, credentials);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthTokensDto
            {
                AccessToken = accessToken,
                RefreshToken = string.Empty
            };
        }

        private async Task<AuthTokensDto> GenerateRs256TokensAsync(User user)
        {
            var privateKey = _configuration["Jwt:Rs256:PrivateKey"];

            if (string.IsNullOrWhiteSpace(privateKey))
            {
                throw new InvalidOperationException("JWT private key is not configured.");
            }

            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);

            var securityKey = new RsaSecurityKey(rsa);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
            var claims = await CreateClaims(user);
            var token = CreateToken(claims, credentials);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthTokensDto
            {
                AccessToken = accessToken,
                RefreshToken = string.Empty
            };
        }

        private async Task<List<Claim>> CreateClaims(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            return claims;
        }

        private JwtSecurityToken CreateToken(List<Claim> claims, SigningCredentials credentials)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new InvalidOperationException("JWT issuer is not configured.");
            }

            if (string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException("JWT audience is not configured.");
            }

            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
                );
        }
    }
}
