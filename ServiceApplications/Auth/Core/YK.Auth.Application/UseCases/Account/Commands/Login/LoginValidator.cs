using FluentValidation;
using YK.Auth.Application.Common.Abstractions.Services.Phone;

namespace YK.Auth.Application.UseCases.Account.Commands.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        private readonly IPhoneNumberService _phoneNumberService;
        public LoginValidator(IPhoneNumberService phoneNumberService)
        {
            _phoneNumberService = phoneNumberService;

            RuleFor(x => x.loginRequest.Phone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Phone is required.")
                .Must(phone => _phoneNumberService.TryNormalize(phone, out _))
                .WithMessage("Phone number is invalid.");

            RuleFor(x => x.loginRequest.Role)
                .NotEmpty()
                .WithMessage("Role is required.");

            RuleFor(x => x.loginRequest.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}
