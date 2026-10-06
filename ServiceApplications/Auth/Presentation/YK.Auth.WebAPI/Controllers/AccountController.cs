using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YK.Auth.Application.Common.Contracts.Authentication;
using YK.Auth.Application.UseCases.Account.Commands.Login;
using YK.Auth.Application.UseCases.Account.Commands.Logout;
using YK.Auth.Application.UseCases.Account.Commands.RefreshToken;
using YK.Auth.Application.UseCases.Account.Commands.Register;

namespace YK.Auth.WebAPI.Controllers
{
    [Route("api/Accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("Login")]
        public async Task<LoginResponseDto> Login(LoginCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);

            return response;
        }

        [AllowAnonymous]
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken ct)
        {
            await _mediator.Send(command, ct);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public async Task<AuthTokensDto> Refresh(RefreshTokenCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            return result;
        }
    }
}
