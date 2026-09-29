using MediatR;
using Microsoft.AspNetCore.Mvc;
using YK.Auth.Application.UseCases.Account.Commands.Login;
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost]
        public async Task<LoginResponseDto> Login(LoginCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);

            return response;
        }
    }
}
