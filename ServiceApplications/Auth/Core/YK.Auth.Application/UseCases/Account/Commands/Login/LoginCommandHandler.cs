using MediatR;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Abstractions.Services.Phone;
using YK.Auth.Application.Common.Exceptions;

namespace YK.Auth.Application.UseCases.Account.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IPhoneNumberService _phoneNumberService;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IIdentityService identityService, IPhoneNumberService phoneNumberService, ITokenService tokenService)
        {
            _identityService = identityService;
            _phoneNumberService = phoneNumberService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var normalizedPhone = _phoneNumberService.NormalizeOrThrow(request.loginRequest.Phone);
            var user = await _identityService.ValidateCredentialsAsync(normalizedPhone, request.loginRequest.Role, request.loginRequest.Password);

            if (user is null)
            {
                throw new UnauthorizedException("Invalid phone, role, or password.");
            }

            var tokens = await _tokenService.GenerateTokensAsync(user, cancellationToken);

            return new LoginResponseDto
            {
                Tokens = tokens
            };
        }
    }
}
