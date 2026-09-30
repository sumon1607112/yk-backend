using YK.Auth.Application.Common.Contracts.Authentication;
using YK.Auth.Domain.Entities.Common.Identity;

namespace YK.Auth.Application.Common.Abstractions.Services.Identity
{
    public interface ITokenService
    {
        Task<AuthTokensDto> GenerateTokensAsync(User user);
    }
}
