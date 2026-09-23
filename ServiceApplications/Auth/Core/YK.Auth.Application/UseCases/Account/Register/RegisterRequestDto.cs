using YK.Auth.Application.Common.Contracts.Identity;

namespace YK.Auth.Application.UseCases.Account.Register
{
    public class RegisterRequestDto
    {
        public CreateUserRequest CreateUserRequest { get; set; } = new();
    }
}
