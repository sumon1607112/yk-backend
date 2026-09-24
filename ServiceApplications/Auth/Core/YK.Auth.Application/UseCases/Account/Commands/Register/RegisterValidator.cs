using FluentValidation;

namespace YK.Auth.Application.UseCases.Account.Commands.Register
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.RegisterRequest.CreateUserRequest.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required.");

            RuleFor(x => x.RegisterRequest.CreateUserRequest.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(
                    x.RegisterRequest.CreateUserRequest.Email))
                .WithMessage("Email address is invalid.");

            RuleFor(x => x.RegisterRequest.CreateUserRequest.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.");

            RuleFor(x => x.RegisterRequest.CreateUserRequest.Role)
                .NotEmpty()
                .WithMessage("Role is required.");
        }
    }
}
