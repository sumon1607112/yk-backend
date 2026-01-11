using FluentValidation;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace YK.Application.UseCases.Users
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
 
            RuleFor(x => x.UserData.Id)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .WithMessage("User Id cannot be less than 0.");

            RuleSet("Credentials", () =>
            {
                RuleFor(x => x.UserData.Email)
                    .NotEmpty().WithMessage("Email address is required.")
                    .EmailAddress().WithMessage("Please provide a valid email format.");

                RuleFor(x => x.UserData.Password)
                    .NotEmpty().WithMessage("Password is required.")
                    .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                    .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]").WithMessage("Password must contain at least one number.");
            });
        }
    }
}
