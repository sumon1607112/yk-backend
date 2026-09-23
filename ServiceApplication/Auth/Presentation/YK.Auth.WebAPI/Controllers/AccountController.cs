using MediatR;
using Microsoft.AspNetCore.Mvc;
using YK.Auth.Application.UseCases.Account.Register;

namespace YK.Auth.WebAPI.Controllers
{
    [Route("api/[controller]")]
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
            await _mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
