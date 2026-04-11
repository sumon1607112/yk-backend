using YK.Application.Abstractions.Services;

namespace YK.Infrastructure.Abstractions.Services
{
    public class AccountService : IAccountService
    {
        public Task<(bool Succeeded, IEnumerable<string> Errors)> CreateUserAsync(string phone, string password, string role)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateTokenAsync(string phone)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExistsAsync(string phone)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateCredentialsAsync(string phone, string password)
        {
            throw new NotImplementedException();
        }
    }
}
