using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Contracts.Authentication;
using YK.Auth.Application.Common.Exceptions;
using YK.Auth.Domain.Entities.Common.Identity;
using YK.Auth.Infrastructure.Abstractions.Persistence.Contexts;

namespace YK.Auth.Infrastructure.Common.Abstractions.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _db;

        public TokenService(IConfiguration configuration, UserManager<User> userManager, ApplicationDbContext db)
        {
            _configuration = configuration;
            _userManager = userManager;
            _db = db;
        }

        public async Task<AuthTokensDto> GenerateTokensAsync(User user, CancellationToken ct = default)
        {
            var (rawRefreshToken, entity) = CreateRefreshToken(user.Id);
            _db.RefreshTokens.Add(entity);
            await _db.SaveChangesAsync(ct);

            return new AuthTokensDto
            {
                AccessToken = await CreateAccessTokenAsync(user),
                RefreshToken = rawRefreshToken
            };
        }

        public async Task<AuthTokensDto> RefreshTokensAsync(string refreshToken, CancellationToken ct = default)
        {
            var hash = Hash(refreshToken);

            var stored = await _db.RefreshTokens
                .Include(x => x.User)
                .SingleOrDefaultAsync(x => x.TokenHash == hash, ct)
                ?? throw new UnauthorizedException("Invalid refresh token.");

            if (stored.RevokedAt is not null)
            {
                // A rotated token was reused: possible theft, so revoke every active token for this user
                await _db.RefreshTokens
                    .Where(x => x.UserId == stored.UserId && x.RevokedAt == null)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.RevokedAt, DateTime.UtcNow), ct);

                throw new UnauthorizedException("Refresh token has been revoked.");
            }

            if (stored.ExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token has expired.");

            var (newRawToken, newEntity) = CreateRefreshToken(stored.UserId);

            stored.RevokedAt = DateTime.UtcNow;
            stored.ReplacedByTokenHash = newEntity.TokenHash;

            _db.RefreshTokens.Add(newEntity);
            await _db.SaveChangesAsync(ct);

            return new AuthTokensDto
            {
                AccessToken = await CreateAccessTokenAsync(stored.User),
                RefreshToken = newRawToken
            };
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            var hash = Hash(refreshToken);
            var stored = await _db.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == hash, ct);

            if (stored is null || !stored.IsActive) return; // idempotent logout

            stored.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        // ---------- helpers ----------

        private (string Raw, RefreshToken Entity) CreateRefreshToken(string userId)
        {
            var days = _configuration.GetValue("Jwt:RefreshTokenDays", 7);
            var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var now = DateTime.UtcNow;

            return (raw, new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = Hash(raw),
                CreatedAt = now,
                ExpiresAt = now.AddDays(days)
            });
        }

        private static string Hash(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        private async Task<string> CreateAccessTokenAsync(User user)
        {
            var claims = await CreateClaims(user);
            var algorithm = _configuration["Jwt:SigningAlgorithm"]?.ToUpperInvariant();

            switch (algorithm)
            {
                case "HS256":
                    {
                        var secretKey = _configuration["Jwt:Hs256:SecretKey"];
                        if (string.IsNullOrWhiteSpace(secretKey))
                            throw new InvalidOperationException("JWT secret key is not configured.");

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                        return WriteToken(claims, new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
                    }
                case "RS256":
                    {
                        var privateKey = _configuration["Jwt:Rs256:PrivateKey"];
                        if (string.IsNullOrWhiteSpace(privateKey))
                            throw new InvalidOperationException("JWT private key is not configured.");

                        using var rsa = RSA.Create();
                        rsa.ImportFromPem(privateKey);
                        var key = new RsaSecurityKey(rsa)
                        {
                            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
                        };
                        return WriteToken(claims, new SigningCredentials(key, SecurityAlgorithms.RsaSha256));
                    }
                default:
                    throw new NotSupportedException($"Signing algorithm '{algorithm}' is not supported.");
            }
        }

        private async Task<List<Claim>> CreateClaims(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in await _userManager.GetRolesAsync(user))
                claims.Add(new Claim("role", role));

            return claims;
        }

        private string WriteToken(List<Claim> claims, SigningCredentials credentials)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(issuer)) throw new InvalidOperationException("JWT issuer is not configured.");
            if (string.IsNullOrWhiteSpace(audience)) throw new InvalidOperationException("JWT audience is not configured.");

            var minutes = _configuration.GetValue("Jwt:AccessTokenMinutes", 15);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
