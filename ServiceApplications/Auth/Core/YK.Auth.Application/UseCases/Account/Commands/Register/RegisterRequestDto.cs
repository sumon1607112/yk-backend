using YK.Auth.Application.Common.Contracts.Identity;

namespace YK.Auth.Application.UseCases.Account.Commands.Register
{
    public class RegisterRequestDto
    {
        public CreateUserRequest CreateUserRequest { get; set; } = new();
    }
}
