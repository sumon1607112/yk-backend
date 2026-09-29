using Microsoft.AspNetCore.Identity;
using YK.Auth.Application.Common.Contracts.Identity;

namespace YK.Auth.Application.Common.Abstractions.Services.Identity
{
    public interface IIdentityService
    {
        Task<bool> UserExistsAsync(string phone, string role);

        Task<(bool Succeeded, IEnumerable<string> Errors)> CreateUserAsync(CreateUserRequest registerRequest);

        Task<IdentityUser?> ValidateCredentialsAsync(string phone, string role, string password);
    }
}
