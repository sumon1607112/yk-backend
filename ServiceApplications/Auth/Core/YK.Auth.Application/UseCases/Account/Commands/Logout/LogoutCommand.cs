using MediatR;

namespace YK.Auth.Application.UseCases.Account.Commands.Logout
{
    public record LogoutCommand(LogoutRequestDto logoutRequest) : IRequest;
}
