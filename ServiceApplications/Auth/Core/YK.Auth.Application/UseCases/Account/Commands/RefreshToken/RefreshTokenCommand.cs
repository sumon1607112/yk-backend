using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using YK.Auth.Application.Common.Contracts.Authentication;
using YK.Auth.Application.UseCases.Account.Commands.Login;

namespace YK.Auth.Application.UseCases.Account.Commands.RefreshToken
{
    public record RefreshTokenCommand(RefreshTokenRequestDto refreshTokenRequest) : IRequest<AuthTokensDto>;
}
