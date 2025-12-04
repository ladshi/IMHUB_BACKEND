using FluentValidation;

namespace IMHub.ApplicationLayer.Features.Organizations.Users.Commands
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email must not exceed 150 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.Username)
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Username));

            RuleFor(x => x.RoleIds)
                .NotEmpty().WithMessage("At least one role must be assigned.");
        }
    }
}


