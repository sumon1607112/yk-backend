using FluentValidation;
using YK.Auth.Application.Common.Abstractions.Services.Phone;
using YK.Auth.Domain.Enums;

namespace YK.Auth.Application.UseCases.Account.Commands.Register
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        private readonly IPhoneNumberService _phoneNumberService;
        private static readonly string[] SelfRegisterRoles =
        [
            nameof(RoleEnum.Buyer),
            nameof(RoleEnum.Seller)
        ];

        public RegisterValidator(IPhoneNumberService phoneNumberService)
        {
            _phoneNumberService = phoneNumberService;

            RuleFor(x => x.registerRequest.CreateUserRequest.Phone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Must(phone => _phoneNumberService.TryNormalize(phone, out _))
                .WithMessage("Phone number is invalid.");

            RuleFor(x => x.registerRequest.CreateUserRequest.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(
                    x.registerRequest.CreateUserRequest.Email))
                .WithMessage("Email address is invalid.");

            RuleFor(x => x.registerRequest.CreateUserRequest.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.");

            RuleFor(x => x.registerRequest.CreateUserRequest.Role)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Role is required.")
                .Must(role => SelfRegisterRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Role must be Buyer or Seller.");
        }
    }
}
