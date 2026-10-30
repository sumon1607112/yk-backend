using MediatR;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Abstractions.Services.Phone;
using YK.Auth.Application.Common.Contracts.Identity;
using YK.Auth.Application.Common.Exceptions;

namespace YK.Auth.Application.UseCases.Account.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IPhoneNumberService _phoneNumberService;
        private readonly IRoleService _roleService;

        public RegisterCommandHandler(IIdentityService identityService, IPhoneNumberService phoneNumberService, IRoleService roleService)
        {
            _identityService = identityService;
            _phoneNumberService = phoneNumberService;
            _roleService = roleService;
        }

        public async Task Handle(RegisterCommand request, CancellationToken ct)
        {
            var createUserRequest = request.registerRequest.CreateUserRequest;
            var normalizedPhone = PhoneNumberServiceExtensions.NormalizeOrThrow(_phoneNumberService, createUserRequest.Phone, "Phone number passed validation but could not be normalized.");

            var userExist = await _identityService.UserExistsAsync(normalizedPhone, createUserRequest.Role);
            var roleExist = await _roleService.RoleExistsAsync(createUserRequest.Role);

            if (userExist)
            {
                throw new ConflictException("A user with this phone number and role already exists.");
            }

            if (!roleExist)
            {
                throw new NotFoundException("The specified role does not exist.");
            }

            var normalizedRequest = new CreateUserRequest
            {
                Phone = normalizedPhone,
                Email = createUserRequest.Email,
                Password = createUserRequest.Password,
                Role = createUserRequest.Role
            };

            await _identityService.CreateUserAsync(normalizedRequest, ct);
        }
    }
}
