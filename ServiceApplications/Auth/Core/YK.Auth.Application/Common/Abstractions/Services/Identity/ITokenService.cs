using YK.Auth.Application.Common.Contracts.Authentication;
using YK.Auth.Domain.Entities.Common.Identity;

namespace YK.Auth.Application.Common.Abstractions.Services.Identity
{
    public interface ITokenService
    {
        Task<AuthTokensDto> GenerateTokensAsync(User user, CancellationToken ct = default);
        Task<AuthTokensDto> RefreshTokensAsync(string refreshToken, CancellationToken ct = default);
        Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    }
}
