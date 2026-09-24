using MediatR;

namespace YK.Auth.Application.UseCases.Account.Commands.Register
{
    public record RegisterCommand(RegisterRequestDto RegisterRequest) : IRequest;
}
