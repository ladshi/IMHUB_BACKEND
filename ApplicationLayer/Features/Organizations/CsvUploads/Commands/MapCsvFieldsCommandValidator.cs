using FluentValidation;

namespace IMHub.ApplicationLayer.Features.Organizations.CsvUploads.Commands
{
    public class MapCsvFieldsCommandValidator : AbstractValidator<MapCsvFieldsCommand>
    {
        public MapCsvFieldsCommandValidator()
        {
            RuleFor(x => x.CsvUploadId)
                .GreaterThan(0).WithMessage("CSV upload ID must be greater than 0.");

            RuleFor(x => x.Mappings)
                .NotNull().WithMessage("Mappings are required.")
                .Must(m => m != null && m.Count > 0).WithMessage("At least one mapping is required.");
        }
    }
}

