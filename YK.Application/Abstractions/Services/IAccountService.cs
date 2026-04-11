using System;
using System.Collections.Generic;
using System.Text;

namespace YK.Application.Abstractions.Services
{
    public interface IAccountService
    {
        //Register
        Task<bool> UserExistsAsync(string phone);
        Task<(bool Succeeded, IEnumerable<string> Errors)> CreateUserAsync(string phone, string password, string role);

        // Login
        Task<bool> ValidateCredentialsAsync(string phone, string password);

        // Shared
        Task<string> GenerateTokenAsync(string phone);
    }
}
