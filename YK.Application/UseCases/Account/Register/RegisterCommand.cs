using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace YK.Application.UseCases.Account.Register
{
    public record RegisterCommand(RegisterRequestDto RegisterRequest) : IRequest<RegisterResponseDto>;
}
