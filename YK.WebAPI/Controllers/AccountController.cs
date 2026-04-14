using MediatR;
using Microsoft.AspNetCore.Mvc;
using YK.Application.UseCases.Account.Register;

namespace YK.WebAPI.Controllers
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
        public async Task<RegisterResponseDto> Register(RegisterCommand command,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command);

            return result ?? new RegisterResponseDto();
        }
    }
}
