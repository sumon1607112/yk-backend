using Microsoft.AspNetCore.Identity;
using YK.Auth.Application.Common.Contracts.Authentication;

namespace YK.Auth.Application.Common.Abstractions.Services.Identity
{
    public interface ITokenService
    {
        Task<AuthTokensDto> GenerateTokensAsync(IdentityUser user);
    }
}
