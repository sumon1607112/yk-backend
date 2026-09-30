using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YK.Auth.Application.UseCases.Role.Commands.CreateRole;

namespace YK.Auth.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Roles")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(CreateRoleRequestDto request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new CreateRoleCommand(request), cancellationToken);

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
