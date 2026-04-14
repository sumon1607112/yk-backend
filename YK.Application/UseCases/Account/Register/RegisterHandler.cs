using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using YK.Application.Abstractions.Services;

namespace YK.Application.UseCases.Account.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponseDto>
    {
        private readonly IAccountService _accountService;

        public RegisterHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var data = request.RegisterRequest;

            if (await _accountService.UserExistsAsync(data.Phone))
                return RegisterResponseDto.Failure(["Phone number already registered."]);

            var roleName = data.Role.ToString();

            var (succeeded, errors) = await _accountService.CreateUserAsync(data.Phone, data.Password, roleName);

            if (!succeeded)
                return RegisterResponseDto.Failure(errors);

            var token = await _accountService.GenerateTokenAsync(data.Phone);

            return RegisterResponseDto.Success(token, data.Phone, roleName);
        }
    }
}
