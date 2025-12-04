using FluentValidation;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Distributions.Commands
{
    public class CreateDistributionCommandValidator : AbstractValidator<CreateDistributionCommand>
    {
        public CreateDistributionCommandValidator()
        {
            RuleFor(x => x.OrganizationId)
                .GreaterThan(0).WithMessage("Organization ID must be greater than 0.");

            RuleFor(x => x.PrinterId)
                .GreaterThan(0).WithMessage("Printer ID must be greater than 0.");
        }
    }
}

