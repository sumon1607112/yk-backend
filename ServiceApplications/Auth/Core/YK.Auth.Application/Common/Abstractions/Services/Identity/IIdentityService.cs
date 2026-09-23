using YK.Auth.Application.Common.Contracts.Identity;
using YK.Auth.Domain.Enums;

namespace YK.Auth.Application.Common.Abstractions.Services.Identity
{
    public interface IIdentityService
    {
        Task<bool> UserExistsAsync(string phone,Role role);

        Task<(bool Succeeded, IEnumerable<string> Errors)> CreateUserAsync(CreateUserRequest registerRequest);

        Task<bool> ValidateCredentialsAsync(string phone,string password);
    }
}
