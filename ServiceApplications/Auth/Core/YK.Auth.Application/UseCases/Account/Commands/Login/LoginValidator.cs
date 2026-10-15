using FluentValidation;

namespace YK.Auth.Application.UseCases.Account.Commands.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.loginRequest.Phone)
                .NotEmpty().WithMessage("Phone is required.");

            RuleFor(x => x.loginRequest.Role)
                .NotEmpty().WithMessage("Role is required.");

            RuleFor(x => x.loginRequest.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
