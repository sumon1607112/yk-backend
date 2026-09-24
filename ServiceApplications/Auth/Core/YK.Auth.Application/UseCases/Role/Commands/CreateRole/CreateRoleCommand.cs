using MediatR;

namespace YK.Auth.Application.UseCases.Role.Commands.CreateRole
{
    public record CreateRoleCommand(CreateRoleRequestDto Request) : IRequest;

}
