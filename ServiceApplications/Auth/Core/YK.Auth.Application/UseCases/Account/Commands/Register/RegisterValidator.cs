using FluentValidation;
using YK.Auth.Domain.Enums;

namespace YK.Auth.Application.UseCases.Account.Commands.Register
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        private static readonly string[] SelfRegisterRoles =
        [
            nameof(RoleEnum.Buyer),
            nameof(RoleEnum.Seller)
        ];

        public RegisterValidator()
        {
            RuleFor(x => x.registerRequest.CreateUserRequest.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required.");

            RuleFor(x => x.registerRequest.CreateUserRequest.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(
                    x.registerRequest.CreateUserRequest.Email))
                .WithMessage("Email address is invalid.");

            RuleFor(x => x.registerRequest.CreateUserRequest.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.");

            RuleFor(x => x.registerRequest.CreateUserRequest.Role)
                .NotEmpty()
                .WithMessage("Role is required.")
                .Must(role => SelfRegisterRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Role must be Buyer or Seller.");
        }
    }
}
