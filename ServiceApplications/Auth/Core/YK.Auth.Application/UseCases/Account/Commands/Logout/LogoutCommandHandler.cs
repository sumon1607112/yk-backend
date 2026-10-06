using MediatR;
using YK.Auth.Application.Common.Abstractions.Services.Identity;

namespace YK.Auth.Application.UseCases.Account.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly ITokenService _tokenService;

        public LogoutCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task Handle(LogoutCommand request, CancellationToken ct)
        {
            await _tokenService.RevokeRefreshTokenAsync(request.logoutRequest.RefreshToken, ct);
        }
    }
}
