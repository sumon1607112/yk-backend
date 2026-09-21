using YK.Application.Common.Contracts.Identity;
using YK.Domain.Enums;

namespace YK.Application.Common.Abstractions.Services.Identity
{
    public interface IIdentityService
    {
        Task<bool> UserExistsAsync(string phone,Role role);

        Task<(bool Succeeded, IEnumerable<string> Errors)> CreateUserAsync(CreateUserRequest registerRequest);

        Task<bool> ValidateCredentialsAsync(string phone,string password);
    }
}
