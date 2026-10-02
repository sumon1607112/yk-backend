using System;
using System.Collections.Generic;
using System.Text;

namespace YK.Auth.Application.UseCases.Account.Commands.RefreshToken
{
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
