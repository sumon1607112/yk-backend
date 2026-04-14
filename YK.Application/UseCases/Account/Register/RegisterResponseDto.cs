using System;
using System.Collections.Generic;
using System.Text;

namespace YK.Application.UseCases.Account.Register
{
    public class RegisterResponseDto
    {
        public bool Succeeded { get; set; }
        public string? Token { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public IEnumerable<string> Errors { get; set; } = [];

        public static RegisterResponseDto Success(string token, string phone, string role)
        {
            return new RegisterResponseDto
            {
                Succeeded = true,
                Token = token,
                Phone = phone,
                Role = role
            };
        }

        public static RegisterResponseDto Failure(IEnumerable<string> errors)
        {
            return new RegisterResponseDto
            {
                Succeeded = false,
                Errors = errors
            };
        }
    }
}
