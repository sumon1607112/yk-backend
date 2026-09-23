using MediatR;

namespace YK.Application.UseCases.Account.Register
{
    public record RegisterCommand(RegisterRequestDto RegisterRequest) : IRequest;
}
