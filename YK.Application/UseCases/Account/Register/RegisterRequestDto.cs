using System;
using System.Collections.Generic;
using System.Text;
using YK.Domain.Enums;

namespace YK.Application.UseCases.Account.Register
{
    public class RegisterRequestDto
    {
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; }
    }
}
