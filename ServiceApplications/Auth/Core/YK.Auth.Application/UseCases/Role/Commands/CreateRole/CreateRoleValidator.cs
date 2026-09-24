using FluentValidation;

namespace YK.Auth.Application.UseCases.Role.Commands.CreateRole
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty()
                .WithMessage("Role name is required.")
                .MaximumLength(50)
                .WithMessage("Role name cannot exceed 50 characters.")
                .Matches("^[a-zA-Z0-9]+$")
                .WithMessage(
                    "Role name can contain only letters and numbers.");
        }
    }
}
