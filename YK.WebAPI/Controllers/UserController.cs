using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YK.Application.UseCases;
using YK.Application.UseCases.Users;

namespace YK.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<UserResponseDto> Upsert(UpdateUserCommand command)
        {
            var result = await _mediator.Send(command);

            return result;
        }
    }
}
