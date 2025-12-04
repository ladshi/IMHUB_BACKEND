using FluentValidation;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Commands
{
    public class CreateSendoutCommandValidator : AbstractValidator<CreateSendoutCommand>
    {
        public CreateSendoutCommandValidator()
        {
            RuleFor(x => x.ContentId)
                .GreaterThan(0).WithMessage("Content ID must be greater than 0.");

            RuleFor(x => x.PrinterId)
                .GreaterThan(0).WithMessage("Printer ID must be greater than 0.");

            RuleFor(x => x.TargetDate)
                .NotEmpty().WithMessage("Target date is required.")
                .Must(BeFutureDate).WithMessage("Target date must be in the future.");
        }

        private bool BeFutureDate(DateTime date)
        {
            return date > DateTime.UtcNow;
        }
    }
}

