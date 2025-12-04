using FluentValidation;
using IMHub.Domain.Enums;

namespace IMHub.ApplicationLayer.Features.Organizations.Sendouts.Commands
{
    public class UpdateSendoutStatusCommandValidator : AbstractValidator<UpdateSendoutStatusCommand>
    {
        public UpdateSendoutStatusCommandValidator()
        {
            RuleFor(x => x.SendoutId)
                .GreaterThan(0).WithMessage("Sendout ID must be greater than 0.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid sendout status.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Notes));
        }
    }
}

