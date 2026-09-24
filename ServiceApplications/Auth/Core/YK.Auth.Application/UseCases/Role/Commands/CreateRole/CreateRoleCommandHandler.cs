using MediatR;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Exceptions;

namespace YK.Auth.Application.UseCases.Role.Commands.CreateRole
{
    internal class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand>
    {
        private readonly IRoleService _roleService;

        public CreateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var roleName = request.Request.Name.Trim();

            var exists = await _roleService.RoleExistsAsync(roleName);

            if (exists)
            {
                throw new ConflictException($"Role '{roleName}' already exists.");
            }

            var result = await _roleService.CreateRoleAsync(roleName);

            if (!result.Succeeded)
            {
                throw new ApplicationException(string.Join("; ", result.Errors.Select(x => x.Description)));
            }
        }
    }
}
