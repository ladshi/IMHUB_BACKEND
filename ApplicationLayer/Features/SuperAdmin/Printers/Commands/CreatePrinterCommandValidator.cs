using FluentValidation;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Printers.Commands
{
    public class CreatePrinterCommandValidator : AbstractValidator<CreatePrinterCommand>
    {
        public CreatePrinterCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Printer name is required.")
                .MaximumLength(100).WithMessage("Printer name must not exceed 100 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.");

            RuleFor(x => x.ApiKey)
                .NotEmpty().WithMessage("API key is required.")
                .MinimumLength(10).WithMessage("API key must be at least 10 characters long.");
        }
    }
}

