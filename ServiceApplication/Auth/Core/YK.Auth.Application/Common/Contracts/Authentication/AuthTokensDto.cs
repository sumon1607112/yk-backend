using System;
using System.Collections.Generic;
using System.Text;

namespace YK.Auth.Application.Common.Contracts.Authentication
{
    public class AuthTokensDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}
