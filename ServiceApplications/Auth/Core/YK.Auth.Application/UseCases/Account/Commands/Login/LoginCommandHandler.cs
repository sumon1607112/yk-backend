using MediatR;
using YK.Auth.Application.Common.Abstractions.Services.Identity;

namespace YK.Auth.Application.UseCases.Account.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.ValidateCredentialsAsync(request.loginRequest.Phone, request.loginRequest.Role, request.loginRequest.Password);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var tokens = await _tokenService.GenerateTokensAsync(user);

            return new LoginResponseDto
            {
                Tokens = tokens
            };
        }
    }
}
