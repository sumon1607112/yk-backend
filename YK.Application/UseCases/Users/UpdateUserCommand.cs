using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace YK.Application.UseCases.Users
{
    public record UpdateUserCommand(UserRequestDto UserData) : IRequest<UserResponseDto>;
}
