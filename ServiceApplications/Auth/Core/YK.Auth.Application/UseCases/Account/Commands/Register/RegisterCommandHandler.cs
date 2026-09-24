using MediatR;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Exceptions;

namespace YK.Auth.Application.UseCases.Account.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IRoleService _roleService;

        public RegisterCommandHandler(IIdentityService identityService, IRoleService roleService)
        {
            _identityService = identityService;
            _roleService = roleService;
        }

        public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var createUserRequest = request.RegisterRequest.CreateUserRequest;

            var userExist = await _identityService.UserExistsAsync(createUserRequest.Phone, createUserRequest.Role);
            var roleExist = await _roleService.RoleExistsAsync(createUserRequest.Role);

            if (userExist)
            {
                throw new ConflictException("A user with this phone number and role already exists.");
            }

            if (!roleExist)
            {
                throw new NotFoundException("The specified role does not exist.");
            }

            var result = await _identityService.CreateUserAsync(createUserRequest);

            if (!result.Succeeded)
            {
                throw new BusinessRuleException(result.Errors);
            }
        }
    }
}
