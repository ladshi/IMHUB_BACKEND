using FluentValidation;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateVersions.Commands
{
    public class CreateTemplateVersionCommandValidator : AbstractValidator<CreateTemplateVersionCommand>
    {
        public CreateTemplateVersionCommandValidator()
        {
            RuleFor(x => x.TemplateId)
                .GreaterThan(0).WithMessage("Template ID must be greater than 0.");

            RuleFor(x => x.VersionNumber)
                .GreaterThan(0).WithMessage("Version number must be greater than 0.");

            RuleFor(x => x.PdfUrl)
                .NotEmpty().WithMessage("PDF URL is required.");

            RuleFor(x => x.DesignJson)
                .Must(BeValidJson).WithMessage("DesignJson must be valid JSON.");
        }

        private bool BeValidJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return false;

            try
            {
                System.Text.Json.JsonDocument.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

