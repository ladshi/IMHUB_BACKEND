using FluentValidation;

namespace IMHub.ApplicationLayer.Features.SuperAdmin.Organizations.Commands
{
    public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
    {
        public CreateOrganizationCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Organization name is required.")
                .MaximumLength(150).WithMessage("Organization name must not exceed 150 characters.");

            RuleFor(x => x.Domain)
                .NotEmpty().WithMessage("Domain is required.")
                .MaximumLength(100).WithMessage("Domain must not exceed 100 characters.")
                .Matches(@"^[a-z0-9-]+$").WithMessage("Domain must contain only lowercase letters, numbers, and hyphens.");

            RuleFor(x => x.TenantCode)
                .NotEmpty().WithMessage("Tenant code is required.")
                .MaximumLength(20).WithMessage("Tenant code must not exceed 20 characters.")
                .Matches(@"^[A-Z0-9]+$").WithMessage("Tenant code must contain only uppercase letters and numbers.");

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

