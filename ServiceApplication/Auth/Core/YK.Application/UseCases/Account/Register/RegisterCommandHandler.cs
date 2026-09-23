using MediatR;
using YK.Application.Common.Abstractions.Services.Identity;
using YK.Application.Common.Exceptions;

namespace YK.Application.UseCases.Account.Register
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
