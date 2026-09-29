using YK.Auth.Application.Common.Contracts.Authentication;

namespace YK.Auth.Application.UseCases.Account.Commands.Login
{
    public class LoginResponseDto
    {
        public AuthTokensDto Tokens { get; set; } = default!;
    }
}
