using YK.Application.Common.Contracts.Identity;

namespace YK.Application.UseCases.Account.Register
{
    public class RegisterRequestDto
    {
        public CreateUserRequest CreateUserRequest { get; set; } = new();
    }
}
