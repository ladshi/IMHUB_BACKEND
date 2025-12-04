using FluentValidation;

namespace IMHub.ApplicationLayer.Features.Organizations.Users.Commands
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("User ID must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Username)
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Username));
        }
    }
}


