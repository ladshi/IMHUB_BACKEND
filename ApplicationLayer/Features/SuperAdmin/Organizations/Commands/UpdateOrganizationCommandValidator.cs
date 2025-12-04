using FluentValidation;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands
{
    public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
    {
        public UpdateOrganizationCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Organization ID must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Organization name is required.")
                .MaximumLength(150).WithMessage("Organization name must not exceed 150 characters.");

            RuleFor(x => x.Domain)
                .NotEmpty().WithMessage("Domain is required.")
                .MaximumLength(100).WithMessage("Domain must not exceed 100 characters.")
                .Matches(@"^[a-z0-9-]+$").WithMessage("Domain must contain only lowercase letters, numbers, and hyphens.");

            RuleFor(x => x.LimitsJson)
                .Must(BeValidJson).WithMessage("LimitsJson must be valid JSON.");
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

