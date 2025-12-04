using FluentValidation;

namespace IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Commands
{
    public class UploadCsvCommandValidator : AbstractValidator<UploadCsvCommand>
    {
        public UploadCsvCommandValidator()
        {
            RuleFor(x => x.TemplateId)
                .GreaterThan(0).WithMessage("Template ID must be greater than 0.");

            RuleFor(x => x.File)
                .NotNull().WithMessage("CSV file is required.")
                .Must(f => f != null && f.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Only CSV files are allowed.")
                .Must(f => f != null && f.Length > 0)
                .WithMessage("File cannot be empty.")
                .Must(f => f != null && f.Length <= 10 * 1024 * 1024) // 10MB max
                .WithMessage("File size must not exceed 10MB.");
        }
    }
}

