using MediatR;

namespace YK.Auth.Application.UseCases.Account.Register
{
    public record RegisterCommand(RegisterRequestDto RegisterRequest) : IRequest;
}
