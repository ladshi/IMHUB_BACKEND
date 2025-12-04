using FluentValidation;

namespace IMHub.ApplicationLayer.Features.Organizations.Templates.TemplateFields.Commands
{
    public class CreateTemplateFieldCommandValidator : AbstractValidator<CreateTemplateFieldCommand>
    {
        public CreateTemplateFieldCommandValidator()
        {
            RuleFor(x => x.TemplatePageId)
                .GreaterThan(0).WithMessage("Template page ID must be greater than 0.");

            RuleFor(x => x.FieldName)
                .NotEmpty().WithMessage("Field name is required.")
                .MaximumLength(100).WithMessage("Field name must not exceed 100 characters.");

            RuleFor(x => x.Width)
                .GreaterThan(0).WithMessage("Width must be greater than 0.");

            RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("Height must be greater than 0.");

            RuleFor(x => x.ValidationRulesJson)
                .Must(BeValidJson).WithMessage("ValidationRulesJson must be valid JSON.");
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

