using MediatR;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Exceptions;

namespace YK.Auth.Application.UseCases.Account.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
    {
        private readonly IIdentityService _identityService;

        public RegisterCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var createUserRequest = request.RegisterRequest.CreateUserRequest;

            var userExist = await _identityService.UserExistsAsync(createUserRequest.Phone, createUserRequest.Role);

            if (userExist)
            {
                throw new ConflictException("A user with this phone number and role already exists.");
            }

            var result = await _identityService.CreateUserAsync(createUserRequest);

            if (!result.Succeeded)
            {
                throw new BusinessRuleException(result.Errors);
            }
        }
    }
}
