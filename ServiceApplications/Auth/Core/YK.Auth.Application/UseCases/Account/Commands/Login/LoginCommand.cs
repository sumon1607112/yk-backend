using MediatR;

namespace YK.Auth.Application.UseCases.Account.Commands.Login
{
    public record LoginCommand(LoginRequestDto loginRequest) : IRequest<LoginResponseDto>;
}
