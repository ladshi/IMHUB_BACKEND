using FluentValidation;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.Commands
{
    public class UpdateTemplateCommandValidator : AbstractValidator<UpdateTemplateCommand>
    {
        public UpdateTemplateCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Template ID must be greater than 0.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Template title is required.")
                .MaximumLength(200).WithMessage("Template title must not exceed 200 characters.");

            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug is required.")
                .MaximumLength(250).WithMessage("Slug must not exceed 250 characters.")
                .Matches(@"^[a-z0-9-]+$").WithMessage("Slug must contain only lowercase letters, numbers, and hyphens.");

            RuleFor(x => x.MetadataJson)
                .Must(BeValidJson).WithMessage("MetadataJson must be valid JSON.");
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

