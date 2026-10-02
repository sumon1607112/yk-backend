using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace YK.Auth.Application.UseCases.Account.Commands.RefreshToken
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required.");
        }
    }
}
