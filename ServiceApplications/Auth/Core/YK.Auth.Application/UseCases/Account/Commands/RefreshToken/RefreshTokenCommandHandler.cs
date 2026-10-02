using MediatR;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Contracts.Authentication;


namespace YK.Auth.Application.UseCases.Account.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthTokensDto>
    {
        private readonly ITokenService _tokenService;
        public RefreshTokenCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<AuthTokensDto> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            var refreshTokenResponse = await _tokenService.RefreshTokensAsync(request.refreshTokenRequest.RefreshToken, ct);  

            return refreshTokenResponse;
        }
    }
}
