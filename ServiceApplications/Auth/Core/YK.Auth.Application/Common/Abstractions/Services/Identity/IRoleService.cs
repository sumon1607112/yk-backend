using Microsoft.AspNetCore.Identity;

namespace YK.Auth.Application.Common.Abstractions.Services.Identity
{
    public interface IRoleService
    {
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> CreateRoleAsync(string roleName);
    }
}
